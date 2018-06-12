using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public interface IUserMapProvider
    {
        User GetUser(string type, string identifier);
        void CreateUserMap(long uid, string type, string identifier);
    }
}
