using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    interface IUserProvider
    {
        // TODO: Declare an interface that allows authentication of various user providers (plain old username/password login, OpenID, etc.)

        User RetrieveUser(string identifier);
    }
}
