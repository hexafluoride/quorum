using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public interface IUserProvider
    {
        User GetUser(long identifier);
        User GetUser(string username);

        long CreateUser(string username);
        bool UpdateUser(long user_id, User user);
    }
}
