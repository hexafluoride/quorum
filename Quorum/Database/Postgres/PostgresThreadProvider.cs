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
    public class PostgresThreadProvider : IThreadProvider
    {
        public PostgresDatabase Database { get; set; }

        public PostgresThreadProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public Thread GetThread(long id)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM threads WHERE id = @id");

            command.Parameters.Add(new NpgsqlParameter("@id", id));

            using (var reader = Database.ExecuteReader(command))
            {
                return FromReader(reader);
            }
        }

        public Thread GetThreadByOpeningPost(long op)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM threads WHERE opening_post = @opening_post");

            command.Parameters.Add(new NpgsqlParameter("@opening_post", op));

            using (var reader = Database.ExecuteReader(command))
            {
                return FromReader(reader);
            }
        }

        public Thread[] GetThreadsByBoardBumpOrdered(long board_id, long offset, long count)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM threads WHERE board = @id ORDER BY last_post DESC LIMIT @limit OFFSET @offset");

            command.Parameters.Add(new NpgsqlParameter("@id", board_id));
            command.Parameters.Add(new NpgsqlParameter("@limit", count));
            command.Parameters.Add(new NpgsqlParameter("@offset", offset));

            var threads = new List<Thread>((int)count);

            using (var reader = Database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    threads.Add(FromReader(reader));
                }
            }

            return threads.ToArray();
        }

        private Thread FromReader(DbDataReader reader)
        {
            if (!reader.HasRows)
                return null;

            reader.Read();

            Thread thread = new Thread();

            thread.Id = (long)reader.GetInt64(0);
            thread.Board = (long)reader.GetInt64(1);
            thread.OpeningPost = (long)reader.GetInt64(2);
            thread.Title = reader.GetString(3);

            return thread;
        }

        public long CreateThread()
        {
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO threads VALUES(DEFAULT, DEFAULT, DEFAULT, DEFAULT, '') RETURNING id");

            return Database.ExecuteNonQuery(command);
        }

        public long CreateThread(long opening_post)
        {
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO threads VALUES(DEFAULT, DEFAULT, @post, @post, '') RETURNING id");

            command.Parameters.AddWithValue("@post", opening_post);

            return Database.ExecuteNonQuery(command);
        }

        public long CreateThread(long opening_post, long board)
        {
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO threads VALUES(DEFAULT, @board, @post, @post, '') RETURNING id");

            command.Parameters.AddWithValue("@board", board);
            command.Parameters.AddWithValue("@post", opening_post);

            return Database.ExecuteNonQuery(command);
        }

        public bool UpdateThread(long thread_id, Thread thread)
        {
            NpgsqlCommand command = new NpgsqlCommand("UPDATE threads SET (board, opening_post, last_post, title) = (@board, @opening_post, @last_post, @title)");

            command.Parameters.AddWithValue("@board", thread.Board);
            command.Parameters.AddWithValue("@opening_post", thread.OpeningPost);
            command.Parameters.AddWithValue("@last_post", thread.LastPost);
            command.Parameters.AddWithValue("@title", thread.Title);

            return Database.ExecuteNonQuery(command) == 1;
        }
    }
}
