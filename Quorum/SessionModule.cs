using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class SessionModule : NancyModule
    {
        // TODO: Finish implementing IUserProvider and use it to authenticate users in this module

        public Dictionary<string, User> Users = new Dictionary<string, User>()
        {
            {"admin", new User("admin") },
            {"user", new User("user") }
        };

        public SessionModule()
        {
            AuthenticationManager.EnableAuthentication(this); // TODO: Manage ISessionProviders separately

            Post("/login", _ => HandleLogin());
        }

        public async Task<object> HandleLogin()
        {
            string error_redirect = Request.Form["error_redirect"] ?? "/";
            string success_redirect = Request.Form["success_redirect"] ?? "/";

            string username = Request.Form["username"] ?? "";

            try
            {
                if(Context.Items.ContainsKey("session"))
                {
                    AuthenticationManager.MainSessionProvider.DestroySession(Context.Items["session"] as Session);
                }

                if(Users.ContainsKey(username))
                {
                    var session = AuthenticationManager.MainSessionProvider.CreateSession(Users[username]);
                    return Response.AsRedirect(success_redirect).WithCookie("_quorum_auth", session.Id, session.ValidUntil);
                }
            }
            catch (Exception ex)
            {
                return Response.AsRedirect(error_redirect);
            }

            return Response.AsRedirect(error_redirect);
        }
    }
}
