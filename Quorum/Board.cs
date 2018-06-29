using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class Board
    {
        public long ParentId { get; set; }
        public BoardParentType ParentType { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Shorthand { get; set; }

        public Board()
        {

        }

        public string GetPath()
        {
            return string.IsNullOrWhiteSpace(Shorthand) ? string.Format("/boards/{0}", Id) : string.Format("/{0}/", Shorthand);
        }
    }

    public class BoardGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public long ParentId { get; set; } // this must be another BoardGroup

        public BoardGroup()
        {

        }
    }

    public enum BoardParentType
    {
        Board, Group
    }
}
