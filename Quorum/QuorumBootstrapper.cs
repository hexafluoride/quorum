using Nancy;
using Nancy.Conventions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quorum
{
    public class QuorumBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.AddDirectory("/assets", "/Content");
            conventions.StaticContentsConventions.AddDirectory("/", "/Pages");
        }
    }
}