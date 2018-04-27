using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get("/", _ => { return string.Format("This is Quorum {0} saying hi!", Utilities.GetVersion()); });
        }
    }
}
