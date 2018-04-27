using Nancy.Hosting.Self;
using NLog;
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
            Log.Info("Quorum version {0}", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

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

            Log.Info("Starting Quorum host, type: {0}", host_type);

            HostMappings[host_type]();
            Thread.Sleep(-1);
        }

        static void StartSelfHosting()
        {
            UriBuilder builder = new UriBuilder();

            builder.Scheme = "http";
            builder.Host = Config.GetString("host.address");
            builder.Port = Config.GetInt("host.port");

            var uri = builder.Uri;

            using (var host = new NancyHost(uri))
            {
                host.Start();
                Log.Info("Listening on {0}", uri);
            }
        }
    }
}
