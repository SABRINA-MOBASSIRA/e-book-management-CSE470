using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EBM.Startup))]
namespace EBM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
