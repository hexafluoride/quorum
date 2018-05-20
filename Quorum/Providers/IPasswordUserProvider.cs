using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public interface IPasswordUserProvider : IUserProvider
    {
        User AttemptAuthenticate(string username, string password);
    }
}
