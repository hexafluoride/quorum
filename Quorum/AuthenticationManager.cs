using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quorum
{
    public class AuthenticationManager
    {
        public static ISessionProvider MainSessionProvider;

        public static void EnableAuthentication(NancyModule module, ISessionProvider provider = null)
        {
            module.Before.AddItemToStartOfPipeline(GetAuthenticationHandler(provider ?? MainSessionProvider));
        }

        public static Func<NancyContext, CancellationToken, Task<Response>> GetAuthenticationHandler(ISessionProvider provider)
        {
            return async (context, cancellation) =>
            {
                if (context.Request.Cookies.ContainsKey("_quorum_auth"))
                {
                    var session_id = context.Request.Cookies["_quorum_auth"];
                    var session = provider.RetrieveSession(session_id);

                    if (session != null)
                    {
                        context.CurrentUser = session.User;
                        context.Items["session"] = session;
                    }
                }

                return null;
            };
        }
    }
}
