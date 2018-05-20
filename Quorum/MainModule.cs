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
            
            AddGetHandler("/", "index");
            AddGetHandler("/login", "login");
            AddGetHandler("/register", "register");
        }

        public void AddGetHandler(string path, string view)
        {
            Get(path, _ => 
            {
                return View[view, new { Navbar = Navbar.Build(Context), User = ((User)Context.CurrentUser) }];
            });
        }
    }
}