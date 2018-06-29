using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database
{
    public static class Extensions
    {
        public static string GetStringSafe(this DbDataReader reader, int column)
        {
            return reader.IsDBNull(column) ? null : reader.GetString(column);
        }
    }
}
