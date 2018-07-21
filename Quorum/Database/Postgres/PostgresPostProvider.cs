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

        public long CreatePost()
        {
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO posts VALUES(DEFAULT, DEFAULT, DEFAULT, '', '', DEFAULT, DEFAULT, '', '') RETURNING id");

            return Database.ExecuteNonQuery(command);
        }

        public bool UpdatePost(long post_id, Post post)
        {
            NpgsqlCommand command = new NpgsqlCommand("UPDATE posts SET " +
                "(author, thread, content, content_type, board, rendered_content, title) = " +
                "(@author, @thread, @content, @content_type, @board, @rendered_content, @title)");

            command.Parameters.AddWithValue("@author", post.Author);
            command.Parameters.AddWithValue("@thread", post.Thread);
            command.Parameters.AddWithValue("@content", post.RawContent);
            command.Parameters.AddWithValue("@content_type", post.Renderer);
            command.Parameters.AddWithValue("@board", post.Board);
            command.Parameters.AddWithValue("@rendered_content", post.RenderedContent);
            command.Parameters.AddWithValue("@title", post.Title);

            return Database.ExecuteNonQuery(command) == 1;
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
