using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ObtenerPesoSAP.Startup))]
namespace ObtenerPesoSAP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
