using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using hass_workstation_service.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MQTTnet.Client.Options;
using hass_workstation_service.Data;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using hass_workstation_service.ServiceHost;
using Serilog;
using Serilog.Formatting.Compact;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.IO;

namespace hass_workstation_service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(new RenderedCompactJsonFormatter(), "logs/log.ndjson")
            .CreateLogger();
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var isService = !(Debugger.IsAttached || args.Contains("--console"));

                    if (isService)
                    {
                        await CreateHostBuilder(args).RunAsServiceAsync();
                    }
                    else
                    {
                        await CreateHostBuilder(args).RunConsoleAsync();
                    }
                }
                else
                {
                    // we only support MS Windows for now
                    throw new NotImplementedException("Your platform is not yet supported");
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddJsonFile("appsettings.json");
                    })
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
                        Name = Environment.MachineName,
                        Identifiers = "hass-workstation-service",
                        Manufacturer = Environment.UserName,
                        Model = Environment.OSVersion.ToString(),
                        Sw_version = "0.0.5"
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
