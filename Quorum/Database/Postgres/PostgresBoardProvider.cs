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
