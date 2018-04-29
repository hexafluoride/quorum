using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database.Postgres
{
    public class PostgresSessionProvider : ISessionProvider
    {
        public PostgresDatabase Database { get; set; }

        internal PostgresSessionProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public Session CreateSession(User user)
        {
            var session = new Session(user);
            
            var command = new NpgsqlCommand("INSERT INTO sessions VALUES(@id, @method, @identifier, @created, @expires)", Database.Connection);

            command.Parameters.Add(new NpgsqlParameter("@id", session.Id));
            command.Parameters.Add(new NpgsqlParameter("@method", "local"));
            command.Parameters.Add(new NpgsqlParameter("@identifier", session.User.Identity.Name));
            command.Parameters.Add(new NpgsqlParameter("@created", session.Created));
            command.Parameters.Add(new NpgsqlParameter("@expires", session.ValidUntil));

            var added = command.ExecuteNonQuery();

            if (added != 1)
                throw new Exception("Inserted " + added + " rows, expected 1");

            return session;
        }

        public void DestroySession(Session session)
        {
            var command = new NpgsqlCommand("DELETE FROM sessions WHERE id = @id", Database.Connection);

            command.Parameters.Add(new NpgsqlParameter("@id", session.Id));

            var added = command.ExecuteNonQuery();

            if (added != 1)
                throw new Exception("Deleted " + added + " rows, expected 1");
        }

        public Session RetrieveSession(string session_id)
        {
            var command = new NpgsqlCommand("SELECT * FROM sessions WHERE id = @id", Database.Connection);

            command.Parameters.Add(new NpgsqlParameter("@id", session_id));

            string username = "";
            Session session = new Session();

            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                    return null;

                reader.Read();

                if (reader.GetString(1) != "local")
                {
                    throw new Exception("Unsupported user authentication method " + reader.GetString(1) + ", was expecting local");
                }

                session.Id = session_id;
                username = reader.GetString(2);
                session.Created = reader.GetDateTime(3);
                session.ValidUntil = reader.GetDateTime(4);
            }

            session.User = Database.UserProvider.RetrieveUser(username);

            return session;
        }
    }
}
