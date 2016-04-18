using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(BYSDN.Startup))]
namespace BYSDN
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
