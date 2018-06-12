using Newtonsoft.Json.Linq;
using NLog;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database.Postgres
{
    public class PostgresDatabase : IDatabase
    {
        // TODO: Kill off connections from the connection pool after a period of inactivity
        //       Currently, the connection pool grows indefinitely as long as all the connections are occupied at the same time a new command is executed
        //       Though, this is somewhat self-regulating as the chance of this happening decreases as more connections are added to the pool
        public List<NpgsqlConnection> Connections = new List<NpgsqlConnection>();

        public PostgresSessionProvider SessionProvider { get; set; }
        public PostgresUserProvider UserProvider { get; set; }
        public PostgresUserMapProvider UserMapProvider { get; set; }
        public PostgresPasswordLoginProvider PasswordLoginProvider { get; set; }

        JObject Settings { get; set; }

        public Logger Log = LogManager.GetCurrentClassLogger();

        public PostgresDatabase(JObject settings)
        {
            Settings = settings;

            SessionProvider = new PostgresSessionProvider(this);
            UserProvider = new PostgresUserProvider(this);
            UserMapProvider = new PostgresUserMapProvider(this);
            PasswordLoginProvider = new PostgresPasswordLoginProvider(this);

            OpenConnection();
        }

        public NpgsqlConnection OpenConnection()
        {
            Log.Debug("Adding new postgres connection to connection pool");
            var connection = new NpgsqlConnection();

            connection.ConnectionString = Settings["connection.string"].Value<string>();
            connection.Open();

            Connections.Add(connection);
            
            return connection;
        }

        public NpgsqlConnection GetFreeConnection()
        {
            var connection = Connections.FirstOrDefault(conn => !conn.FullState.HasFlag(ConnectionState.Executing) && !conn.FullState.HasFlag(ConnectionState.Fetching)) ?? OpenConnection();
            return connection;
        }

        public DbDataReader ExecuteReader(DbCommand command)
        {
            lock (Connections)
            {
                var connection = GetFreeConnection();

                command.Connection = connection;
                var reader = command.ExecuteReader();

                return reader;
            }
        }

        public int ExecuteNonQuery(DbCommand command)
        {
            lock (Connections)
            {
                var connection = GetFreeConnection();

                command.Connection = connection;
                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(DbCommand command)
        {
            lock (Connections)
            {
                var connection = GetFreeConnection();

                command.Connection = connection;
                return command.ExecuteScalar();
            }
        }
    }
}
