using Npgsql;
using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database.Postgres
{
    public class PostgresUserProvider : IUserProvider
    {
        public PostgresDatabase Database { get; set; }

        internal PostgresUserProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public User GetUser(long identifier)
        {
            var command = new NpgsqlCommand("SELECT * FROM users WHERE uid = @uid");

            command.Parameters.Add(new NpgsqlParameter("@uid", identifier));

            using (var reader = Database.ExecuteReader(command))
            {
                if (!reader.HasRows)
                    throw new Exception("Couldn't find user with identifier " + identifier);

                reader.Read();

                return new User(reader.GetInt64(0), reader.GetString(1));
            }
        }

        public User GetUser(string username)
        {
            var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username");

            command.Parameters.Add(new NpgsqlParameter("@username", username));
            using (var reader = Database.ExecuteReader(command))
            {
                if (!reader.HasRows)
                    return null;

                reader.Read();

                var user = new User(reader.GetInt64(0), reader.GetString(1));

                return user;
            }
        }

        public User CreateUser(string username)
        {
            if (GetUser(username) != null)
                throw new Exception("User already exists.");

            var command = new NpgsqlCommand("INSERT INTO users VALUES(DEFAULT, @username)");

            command.Parameters.Add(new NpgsqlParameter("@username", username));

            var added = Database.ExecuteNonQuery(command);

            if (added != 1)
                throw new Exception("Inserted " + added + " rows, expected 1");

            return GetUser(username);
        }
    }
}
