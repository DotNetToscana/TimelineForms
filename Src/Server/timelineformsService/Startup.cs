using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(timelineformsService.Startup))]

namespace timelineformsService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}