using Nancy.Hosting.Self;
using Newtonsoft.Json.Linq;
using NLog;
using Quorum.Database.Postgres;
using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quorum
{
    class Program
    {
        static Logger Log = LogManager.GetCurrentClassLogger();
        static Dictionary<string, Action> HostMappings = new Dictionary<string, Action>()
        {
            {"self", StartSelfHosting }
        };

        static void Main(string[] args)
        {
            Log.Info("Quorum version {0}", Utilities.GetVersion());

            var config_location = args.LastOrDefault() ?? "config.json";
            Config.Load(config_location);

            Log.Info("Loaded configuration from \"{0}\"", config_location);

            var host_type = Config.GetString("host.type");
            
            if(!HostMappings.ContainsKey(host_type))
            {
                Log.Error("Invalid host type \"{0}\" specified, cannot continue.", host_type);
                Log.Error("Valid host types are: {{0}}", string.Join(", ", HostMappings.Keys));
                return;
            }

            var database_type = Config.GetString("database.type");

            switch(database_type)
            {
                case "postgres":
                    var db = new PostgresDatabase(Config.GetValue<JObject>("database"));
                    AuthenticationManager.AddProvider<ISessionProvider>(db.SessionProvider);
                    AuthenticationManager.AddProvider<IUserProvider>(db.UserProvider);
                    AuthenticationManager.AddProvider<IUserMapProvider>(db.UserMapProvider);
                    AuthenticationManager.AddProvider<IPasswordLoginProvider>(db.PasswordLoginProvider);
                    AuthenticationManager.AddProvider<IBoardProvider>(db.BoardProvider);
                    AuthenticationManager.AddProvider<IThreadProvider>(db.ThreadProvider);
                    AuthenticationManager.AddProvider<IPostProvider>(db.PostProvider);
                    break;
                case "":
                    Log.Error("Please specify a database type and options using \"database.type\" and \"database\" in the configuration.");
                    return;
                default:
                    Log.Error("Unknown database type \"\"", database_type);
                    return;
            }

            Log.Info("Connected to {0} database", database_type);
            Log.Info("Starting Quorum host, type: {0}", host_type);

            HostMappings[host_type]();
        }

        static void StartSelfHosting()
        {
            HostConfiguration host_config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };

            UriBuilder builder = new UriBuilder();

            builder.Scheme = "http";
            builder.Host = Config.GetString("host.address");
            builder.Port = Config.GetInt("host.port");

            var uri = builder.Uri;

            using (var host = new NancyHost(host_config, uri))
            {
                host.Start();
                Log.Info("Listening on {0}", uri);

                System.Threading.Thread.Sleep(-1);
            }
        }
    }
}
