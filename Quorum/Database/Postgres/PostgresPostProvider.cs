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
    public class PostgresPostProvider : IPostProvider
    {
        public PostgresDatabase Database { get; set; }

        public PostgresPostProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public Post GetPost(long id)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM posts WHERE id = @id");

            command.Parameters.Add(new NpgsqlParameter("@id", id));

            using (var reader = Database.ExecuteReader(command))
            {
                reader.Read();
                return FromReader(reader);
            }
        }

        public Post[] GetPostsByThread(long thread_id, long offset, long count)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM posts WHERE thread = @id LIMIT @limit OFFSET @offset");

            command.Parameters.Add(new NpgsqlParameter("@id", thread_id));
            command.Parameters.Add(new NpgsqlParameter("@limit", count));
            command.Parameters.Add(new NpgsqlParameter("@offset", offset));

            var posts = new List<Post>((int)count);

            using (var reader = Database.ExecuteReader(command))
            {
                while(reader.Read())
                {
                    posts.Add(FromReader(reader));
                }
            }

            return posts.ToArray();
        }

        private Post FromReader(DbDataReader reader)
        {
            if (!reader.HasRows)
                return null;

            Post post = new Post();

            post.Id = (long)reader.GetInt64(0);
            post.Author = (long)reader.GetInt64(1);

            post.Created = reader.GetDateTime(5);
            post.Thread = (long)reader.GetInt64(2);
            post.Board = (long)reader.GetInt64(6);

            post.RawContent = reader.GetStringSafe(3);
            post.RenderedContent = reader.GetStringSafe(7);
            post.Renderer = reader.GetStringSafe(4);

            post.Title = reader.GetStringSafe(8);

            return post;
        }
    }
}
