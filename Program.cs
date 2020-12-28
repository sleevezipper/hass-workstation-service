using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using hass_desktop_service.Communication;
using Microsoft.Extensions.Configuration;
using MQTTnet.Client.Options;
using hass_desktop_service.Data;

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
                    IConfiguration configuration = hostContext.Configuration;
                    IConfigurationSection mqttSection = configuration.GetSection("MqttBroker");
                    var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(mqttSection.GetValue<string>("Host"))
                    // .WithTls()
                    .WithCredentials(mqttSection.GetValue<string>("Username"), mqttSection.GetValue<string>("Password"))
                    .Build();
                    var deviceConfig = new DeviceConfigModel
                    {
                        Name = "hass-workstation-service3",
                        //TODO: make this more dynamic
                        Identifiers = "hass-workstation-service_unique4",
                        Sw_version = "0.0.4"
                    };
                    services.AddSingleton(configuration);
                    services.AddSingleton(deviceConfig);
                    services.AddSingleton(mqttClientOptions);
                    services.AddSingleton<ConfiguredSensorsService>();
                    services.AddSingleton<MqttPublisher>();
                    services.AddHostedService<Worker>();
                });
    }
}
