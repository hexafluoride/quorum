using Nancy.ViewEngines.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public abstract class QuorumRazorViewBase : NancyRazorViewBase
    {
        public string QuorumVersion { get => Utilities.GetVersion(); }
        public string ForumName { get => Config.GetString("forum.name"); }
        public Session Session { get => Context.Items.ContainsKey("session") ? (Session)Context.Items["session"] : null; }
    }
}
