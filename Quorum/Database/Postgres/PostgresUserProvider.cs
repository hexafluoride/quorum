using Npgsql;
using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

                return FromReader(reader);
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

                return FromReader(reader);
            }
        }

        public long CreateUser(string username)
        {
            if (GetUser(username) != null)
                throw new Exception("User already exists.");

            var command = new NpgsqlCommand("INSERT INTO users VALUES(DEFAULT, @username) RETURNING uid");

            command.Parameters.Add(new NpgsqlParameter("@username", username));

            return Database.ExecuteNonQuery(command);
        }

        public bool UpdateUser(long user_id, User user)
        {
            NpgsqlCommand command = new NpgsqlCommand("UPDATE users SET (username, bio) = (@username, @bio)");

            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@bio", user.Bio);

            return Database.ExecuteNonQuery(command) == 1;
        }

        private User FromReader(DbDataReader reader)
        {
            if (!reader.HasRows)
                return null;

            User user = new User(reader.GetString(1));
            user.Identifier = reader.GetInt64(0);
            user.Bio = reader.GetString(2);

            return user;
        }
    }
}
