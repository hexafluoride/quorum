using Npgsql;
using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database.Postgres
{
    public class PostgresUserMapProvider : IUserMapProvider
    {
        public PostgresDatabase Database { get; set; }

        public PostgresUserMapProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public User GetUser(string type, string identifier)
        {
            var command = new NpgsqlCommand("SELECT uid FROM user_map WHERE method = @method AND identifier = @identifier");

            command.Parameters.Add(new NpgsqlParameter("@method", type));
            command.Parameters.Add(new NpgsqlParameter("@identifier", identifier));

            using (var reader = Database.ExecuteReader(command))
            {
                if (!reader.HasRows)
                    return null;

                reader.Read();

                return Database.UserProvider.GetUser(reader.GetInt64(0));
            }
        }

        public void CreateUserMap(long uid, string type, string identifier)
        {
            var command = new NpgsqlCommand("INSERT INTO user_map VALUES(@method, @identifier, @uid)");

            command.Parameters.Add(new NpgsqlParameter("@method", type));
            command.Parameters.Add(new NpgsqlParameter("@identifier", identifier));
            command.Parameters.Add(new NpgsqlParameter("@uid", uid));

            var added = Database.ExecuteNonQuery(command);

            if (added != 1)
                throw new Exception("Inserted " + added + " rows, expected 1");
        }
    }
}
