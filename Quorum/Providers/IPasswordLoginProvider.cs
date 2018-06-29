using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public interface IPasswordLoginProvider
    {
        void CreateLogin(User user, string username, string password, string email);
        User AttemptAuthenticate(string username, string password);
    }
}
