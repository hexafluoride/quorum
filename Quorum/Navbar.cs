using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public static class Navbar
    {
        public static NavbarElement[] Build(string path)
        {
            return new[]
            {
                new NavbarElement("Home", "/", path)
            };
        }
    }

    public class NavbarElement
    {
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public NavbarElement(bool active, string name, string path)
        {
            Active = active;
            Name = name;
            Path = path;
        }

        public NavbarElement(string name, string path, string current_path) :
            this(path == current_path, name, path)
        {

        }
    }
}
