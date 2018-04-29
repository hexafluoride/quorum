using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class User : ClaimsPrincipal
    {
        public User(string name)
        {
            AddIdentity(new GenericIdentity(name));
        }

        public override string ToString()
        {
            return Identity.Name;
        }
    }
}
