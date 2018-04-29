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

            Get("/logout", _ => HandleLogout());
            Post("/logout", _ => HandleLogout());
        }

        public async Task<object> HandleLogout()
        {
            string error_redirect = Request.Form["error_redirect"].Value ?? "/";
            string success_redirect = Request.Form["success_redirect"].Value ?? "/";

            try
            {
                if (Context.Items.ContainsKey("session"))
                {
                    var session = Context.Items["session"] as Session;

                    AuthenticationManager.MainSessionProvider.DestroySession(session);
                    return Response.AsRedirect(success_redirect).WithCookie("_quorum_auth", "", new DateTime(1970, 1, 1));
                }
            }
            catch (Exception ex)
            {
                return Response.AsRedirect(error_redirect);
            }

            return Response.AsRedirect(error_redirect);
        }

        public async Task<object> HandleLogin()
        {
            string error_redirect = Request.Form["error_redirect"].Value ?? "/";
            string success_redirect = Request.Form["success_redirect"].Value ?? "/";

            string username = Request.Form["username"].Value ?? "";

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
