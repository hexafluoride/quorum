using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public static class Navbar
    {
        public static NavbarElement[] Build(NancyContext context)
        {
            var path = context.Request.Path;

            var ret = new[]
            {
                new NavbarElement("Home", "/", path)
            };

            if(context.CurrentUser != null)
            {
                ret = ret.Concat(new[]
                {
                    new NavbarElement("Logout", "/logout", path, NavbarElementAlignment.Right)
                }).ToArray();
            }
            else
            {
                ret = ret.Concat(new[]
                {
                    new NavbarElement("Log in", "/login", path, NavbarElementAlignment.Right),
                    new NavbarElement("Sign up", "/register", path, NavbarElementAlignment.Right)
                }).ToArray();
            }

            return ret;
        }
    }

    public class NavbarElement
    {
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public NavbarElementAlignment Alignment { get; set; }

        public NavbarElement(bool active, string name, string path, NavbarElementAlignment alignment = NavbarElementAlignment.Left)
        {
            Active = active;
            Name = name;
            Path = path;

            Alignment = alignment;
        }

        public NavbarElement(string name, string path, string current_path, NavbarElementAlignment alignment = NavbarElementAlignment.Left) :
            this(path == current_path, name, path, alignment)
        {

        }
    }

    public enum NavbarElementAlignment
    {
        Left,
        Right
    }
}
