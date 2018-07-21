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
        public long Identifier { get; set; }
        public string Username { get => FindFirst("Username")?.Value; }
        public string Bio { get; set; }

        public User(long identifier, string username)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(username, "quorum_auth"));
            identity.AddClaim(new Claim("Username", username));

            AddIdentity(identity);

            Identifier = identifier;
        }

        public User(string name)
            : this(-1, name)
        {
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
