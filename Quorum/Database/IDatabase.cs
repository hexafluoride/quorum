using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Database
{
    public interface IDatabase
    {
        DbDataReader ExecuteReader(DbCommand command);
        int ExecuteNonQuery(DbCommand command);
        object ExecuteScalar(DbCommand command);
    }
}
