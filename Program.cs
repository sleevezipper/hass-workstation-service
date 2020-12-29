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
using Serilog;
using Serilog.Formatting.Compact;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.IO;
using Microsoft.Win32;

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
                    if (args.Contains("--autostart=true"))
                    {
                        Log.Logger.Information("configuring autostart");
                        // The path to the key where Windows looks for startup applications
                        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                        //Path to launch shortcut
                        string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\hass-workstation-service\hass-workstation-service.appref-ms";

                        rkApp.SetValue("hass-workstation-service", startPath);
                        rkApp.Close();
                    }
                    else if (args.Contains("--autostart=false"))
                    {
                        Log.Logger.Information("removing autostart");
                        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                        rkApp.DeleteSubKey("hass-workstation-service");
                        rkApp.Close();
                    }

                    await CreateHostBuilder(args).RunConsoleAsync();

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
                        Sw_version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
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
