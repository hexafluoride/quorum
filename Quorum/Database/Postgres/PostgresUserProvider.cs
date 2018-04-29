using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database.Postgres
{
    public class PostgresUserProvider : IPasswordUserProvider
    {
        public PostgresDatabase Database { get; set; }

        internal PostgresUserProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public User RetrieveUser(string identifier)
        {
            if(!int.TryParse(identifier, out int id))
            {
                throw new Exception("Unexpected user identifier " + identifier);
            }

            var command = new NpgsqlCommand("SELECT * FROM users WHERE id = @id", Database.Connection);

            command.Parameters.Add(new NpgsqlParameter("@id", id));
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                    throw new Exception("Couldn't find user for identifier " + id);

                reader.Read();

                return new User(reader.GetString(0), "local", reader.GetInt32(3).ToString());
            }
        }

        public User FindUserByUsername(string username)
        {
            var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", Database.Connection);

            command.Parameters.Add(new NpgsqlParameter("@username", username));
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                    return null;

                reader.Read();

                var user = new User(reader.GetString(0), "local", reader.GetInt32(3).ToString());

                return user;
            }
        }

        public User AttemptAuthenticate(string username, string password)
        {
            var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", Database.Connection);

            command.Parameters.Add(new NpgsqlParameter("@username", username));
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                    return null;

                reader.Read();

                var user = new User(reader.GetString(0), "local", reader.GetInt32(3).ToString());

                if (Pbkdf2CryptConverter.Compare(reader.GetString(1), password))
                    return user;

                return null;
            }
        }

        public void CreateUser(string username, string password, string email)
        {
            if (FindUserByUsername(username) != null)
                throw new Exception("User already exists.");

            var command = new NpgsqlCommand("INSERT INTO users VALUES(@username, @password, @email)", Database.Connection);
            
            command.Parameters.Add(new NpgsqlParameter("@username", username));
            command.Parameters.Add(new NpgsqlParameter("@password", Pbkdf2CryptConverter.Encrypt(password)));
            command.Parameters.Add(new NpgsqlParameter("@email", email));

            var added = command.ExecuteNonQuery();

            if (added != 1)
                throw new Exception("Inserted " + added + " rows, expected 1");
        }
    }
}
