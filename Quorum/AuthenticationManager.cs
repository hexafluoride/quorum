using Nancy;
using Quorum.Providers;
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
        public static Dictionary<Type, object> MainProviders = new Dictionary<Type, object>();

        public static T GetProvider<T>()
        {
            return (T)MainProviders.First(t => t.Key == typeof(T)).Value;
        }

        public static void AddProvider<T>(T provider)
        {
            MainProviders[typeof(T)] = provider;
        }

        public static void EnableAuthentication(NancyModule module, ISessionProvider provider = null)
        {
            module.Before.AddItemToStartOfPipeline(GetAuthenticationHandler(provider ?? GetProvider<ISessionProvider>()));
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
