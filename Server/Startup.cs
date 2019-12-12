using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Server.Startup))]

namespace Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCors(CorsOptions.AllowAll);
            app.Map("/signalr", map =>
            {
                var hubConfigration = new HubConfiguration
                {
                    EnableJSONP = true,
                    EnableJavaScriptProxies = false
                };
                map.RunSignalR(hubConfigration);
            });
        }
    }
}
