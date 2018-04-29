using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database.Postgres
{
    public class PostgresDatabase : IDatabase
    {
        public NpgsqlConnection Connection = new NpgsqlConnection();

        public PostgresSessionProvider SessionProvider { get; set; }
        public PostgresUserProvider UserProvider { get; set; }

        public PostgresDatabase(JObject settings)
        {
            Connection.ConnectionString = settings["connection.string"].Value<string>();
            Connection.Open();

            SessionProvider = new PostgresSessionProvider(this);
            UserProvider = new PostgresUserProvider(this);
        }
    }
}
