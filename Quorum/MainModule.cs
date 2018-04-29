using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            AuthenticationManager.EnableAuthentication(this);

            Get("/", _ => {
                return string.Format("This is Quorum {0} saying hi! You are: {1}", Utilities.GetVersion(), this.Context.CurrentUser?.Identity?.Name); });
        }
    }
}