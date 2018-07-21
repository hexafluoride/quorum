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
    public class PostgresBoardProvider : IBoardProvider
    {
        public PostgresDatabase Database { get; set; }

        public PostgresBoardProvider(PostgresDatabase database)
        {
            Database = database;
        }

        public Board GetBoard(long id)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM boards WHERE id = @id");

            command.Parameters.Add(new NpgsqlParameter("@id", id));

            using (var reader = Database.ExecuteReader(command))
            {
                reader.Read();
                return FromReader(reader);
            }
        }

        public Board[] GetBoardsUnderBoardGroup(long group_id)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM boards WHERE parent = @id AND parent_type = 'group'");

            command.Parameters.Add(new NpgsqlParameter("@id", group_id));

            List<Board> boards = new List<Board>();

            using (var reader = Database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    boards.Add(FromReader(reader));
                }
            }

            return boards.ToArray();
        }

        public Board[] GetBoardsUnderBoard(long board_id)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM boards WHERE parent = @id AND parent_type = 'board'");

            command.Parameters.Add(new NpgsqlParameter("@id", board_id));

            List<Board> boards = new List<Board>();

            using (var reader = Database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    boards.Add(FromReader(reader));
                }
            }

            return boards.ToArray();
        }

        public Board[] GetAllBoards()
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM boards");

            List<Board> boards = new List<Board>();

            using (var reader = Database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    boards.Add(FromReader(reader));
                }
            }

            return boards.ToArray();
        }

        public BoardGroup[] GetBoardGroupsUnderBoardGroup(long group_id)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM board_groups WHERE parent = @id");

            command.Parameters.Add(new NpgsqlParameter("@id", group_id));

            List<BoardGroup> groups = new List<BoardGroup>();

            using (var reader = Database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    groups.Add(GroupFromReader(reader));
                }
            }

            return groups.ToArray();
        }

        public BoardGroup GetBoardGroup(long id)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM board_groups WHERE id = @id");

            command.Parameters.Add(new NpgsqlParameter("@id", id));

            using (var reader = Database.ExecuteReader(command))
            {
                reader.Read();
                return GroupFromReader(reader);
            }
        }

        public BoardGroup[] GetAllBoardGroups()
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM board_groups");

            List<BoardGroup> groups = new List<BoardGroup>();

            using (var reader = Database.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    groups.Add(GroupFromReader(reader));
                }
            }

            return groups.ToArray();
        }

        public long CreateBoard(string name, BoardParentType parent_type, long parent)
        {
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO boards VALUES(DEFAULT, @name, DEFAULT, @type, @parent, DEFAULT) RETURNING id");

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@type", parent_type);
            command.Parameters.AddWithValue("@parent", parent);

            return Database.ExecuteNonQuery(command);
        }

        public bool UpdateBoard(long id, Board board)
        {
            NpgsqlCommand command = new NpgsqlCommand("UPDATE boards SET (name, description, parent_type, parent, alias) = (@name, @description, @parent_type, @parent, @alias)");

            command.Parameters.AddWithValue("@name", board.Name);
            command.Parameters.AddWithValue("@description", board.Description);
            command.Parameters.AddWithValue("@parent_type", board.ParentType);
            command.Parameters.AddWithValue("@parent", board.ParentId);
            command.Parameters.AddWithValue("@alias", board.Shorthand);

            return Database.ExecuteNonQuery(command) == 1;
        }

        public long CreateBoardGroup(string name)
        {
            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO board_groups VALUES (DEFAULT, @name, DEFAULT, -1) RETURNING id");

            command.Parameters.AddWithValue("@name", name);

            return Database.ExecuteNonQuery(command);
        }

        public bool UpdateBoardGroup(long id, BoardGroup group)
        {
            NpgsqlCommand command = new NpgsqlCommand("UPDATE board_groups SET (name, description, parent) = (@name, @description, @parent)");

            command.Parameters.AddWithValue("@name", group.Name);
            command.Parameters.AddWithValue("@description", group.Description);
            command.Parameters.AddWithValue("@parent", group.ParentId);

            return Database.ExecuteNonQuery(command) == 1;
        }

        private Board FromReader(DbDataReader reader)
        {
            if (!reader.HasRows)
                return null;

            Board board = new Board();

            board.Id = (long)reader.GetInt64(0);
            board.Name = reader.GetStringSafe(1);
            board.Description = reader.GetStringSafe(2);
            board.ParentType = reader.GetFieldValue<BoardParentType>(3);
            board.ParentId = (long)reader.GetInt64(4);
            board.Shorthand = reader.GetStringSafe(5);

            return board;
        }

        private BoardGroup GroupFromReader(DbDataReader reader)
        {
            if (!reader.HasRows)
                return null;

            BoardGroup group = new BoardGroup();

            group.Id = (long)reader.GetInt64(0);
            group.Name = reader.GetStringSafe(1);
            group.Description = reader.GetStringSafe(2);
            group.ParentId = (long)reader.GetInt64(3);

            return group;
        }
    }
}
