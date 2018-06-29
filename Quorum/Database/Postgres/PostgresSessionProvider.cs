using Npgsql;
using NpgsqlTypes;
using Quorum.Providers;
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
            
            var command = new NpgsqlCommand("INSERT INTO sessions VALUES(@id, @created, @expires, @uid)");

            command.Parameters.Add(new NpgsqlParameter("@id", session.Id));
            command.Parameters.Add(new NpgsqlParameter("@created", session.Created));
            command.Parameters.Add(new NpgsqlParameter("@expires", session.ValidUntil));
            command.Parameters.Add(new NpgsqlParameter("@uid", user.Identifier));

            var added = Database.ExecuteNonQuery(command);

            if (added != 1)
                throw new Exception("Inserted " + added + " rows, expected 1");

            return session;
        }

        public void DestroySession(Session session)
        {
            var command = new NpgsqlCommand("DELETE FROM sessions WHERE id = @id");

            command.Parameters.Add(new NpgsqlParameter("@id", session.Id));

            var added = Database.ExecuteNonQuery(command);

            if (added != 1)
                throw new Exception("Deleted " + added + " rows, expected 1");
        }

        public Session RetrieveSession(string session_id)
        {
            var command = new NpgsqlCommand("SELECT * FROM sessions WHERE id = @id");

            command.Parameters.Add(new NpgsqlParameter("@id", session_id));
            
            Session session = new Session();

            using (var reader = Database.ExecuteReader(command))
            {
                if (!reader.HasRows)
                    return null;

                reader.Read();

                session.Id = session_id;
                session.User = Database.UserProvider.GetUser(reader.GetInt64(3));
                session.Created = reader.GetDateTime(1);
                session.ValidUntil = reader.GetDateTime(2);
            }

            return session;
        }
    }
}
