using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SPA.Startup))]
namespace SPA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
