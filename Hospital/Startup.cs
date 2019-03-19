using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using NLog;
using Owin;

[assembly: OwinStartup(typeof(Hospital.Startup))]

namespace Hospital
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("configuring autentification");
            ConfigureAuth(app);
        }
    }
}
