using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using hass_workstation_service.Communication.NamedPipe;
using JKang.IpcServiceFramework.Client;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using System.Reactive.Linq;
using UserInterface.ViewModels;
using System.Security;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using UserInterface.Util;

namespace UserInterface.Views
{
    public class AppInfo : UserControl
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;

        public AppInfo()
        {
            this.InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<ServiceContractInterfaces>("broker", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<ServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<ServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("broker");


            DataContext = new BackgroundServiceSettingsViewModel();
            Ping();
        }
        public async void Ping() {
            while (true)
            {
                try
                {
                    var result = await this.client.InvokeAsync(x => x.Ping("ping"));
                    if (result == "pong")
                    {
                        ((BackgroundServiceSettingsViewModel)this.DataContext).UpdateStatus(true, "All good");
                    }
                    else
                    {
                        ((BackgroundServiceSettingsViewModel)this.DataContext).UpdateStatus(false, "Not running");
                    }
                }
                catch (System.Exception)
                {
                    ((BackgroundServiceSettingsViewModel)this.DataContext).UpdateStatus(false, "Not running");
                }

                var autostartresult = await this.client.InvokeAsync(x => x.IsAutoStartEnabled());
                ((BackgroundServiceSettingsViewModel)this.DataContext).UpdateAutostartStatus(autostartresult);

                await Task.Delay(1000);
            }
        }

        public void Github(object sender, RoutedEventArgs args)
        {
            BrowserUtil.OpenBrowser("https://github.com/sleevezipper/hass-workstation-service");
        }

        public void Discord(object sender, RoutedEventArgs args)
        {
            BrowserUtil.OpenBrowser("https://discord.gg/VraYT2N3wd");
        }

        public void EnableAutostart(object sender, RoutedEventArgs args)
        {
            this.client.InvokeAsync(x => x.EnableAutostart(true));
        }
        public void DisableAutostart(object sender, RoutedEventArgs args)
        {
            this.client.InvokeAsync(x => x.EnableAutostart(false));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
