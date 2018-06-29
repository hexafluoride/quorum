using Npgsql;
using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database.Postgres
{
    public class PostgresPasswordLoginProvider : IPasswordLoginProvider
    {
        public PostgresDatabase Database { get; set; }

        public PostgresPasswordLoginProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public void CreateLogin(User user, string username, string password, string email = "")
        {
            var command = new NpgsqlCommand("INSERT INTO logins VALUES(@username, @password, @email)");

            command.Parameters.Add(new NpgsqlParameter("@username", username));
            command.Parameters.Add(new NpgsqlParameter("@password", Pbkdf2CryptConverter.Encrypt(password)));
            command.Parameters.Add(new NpgsqlParameter("@email", email));

            var inserted = Database.ExecuteNonQuery(command);

            if (inserted != 1)
                throw new Exception("Inserted " + inserted + " rows, expected 1");
        }

        public User AttemptAuthenticate(string username, string password)
        {
            var command = new NpgsqlCommand("SELECT * FROM logins WHERE username = @username");

            command.Parameters.Add(new NpgsqlParameter("@username", username));
            using (var reader = Database.ExecuteReader(command))
            {
                if (!reader.HasRows)
                    return null;

                reader.Read();

                //var user = new User(reader.GetString(0), "local", reader.GetInt32(3).ToString());

                if (Pbkdf2CryptConverter.Compare(reader.GetString(1), password))
                    return Database.UserMapProvider.GetUser("local", username);

                return null;
            }
        }
    }
}
