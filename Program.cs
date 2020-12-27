using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using hass_desktop_service.StateDetectors.Windows.Fullscreen;

namespace hass_desktop_service
{
    public class Program
    {
        public static void Main(string[] args)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CreateHostBuilder(args).Build().Run();
            }
            else
            {
                // we only support MS Windows for now
                throw new NotImplementedException("Your platform is not yet supported");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<UserNotificationStateDetector>();
                    services.AddHostedService<Worker>();
                });
    }
}
