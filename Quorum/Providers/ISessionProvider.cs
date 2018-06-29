using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public interface ISessionProvider
    {
        Session CreateSession(User user);
        void DestroySession(Session session);

        Session RetrieveSession(string session);
    }
}
