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
        public string Username { get => FindFirst("Username")?.Value; }

        public User(string name, string auth_type, string identifier)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(identifier, auth_type));
            identity.AddClaim(new Claim("Username", name));

            AddIdentity(identity);
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
