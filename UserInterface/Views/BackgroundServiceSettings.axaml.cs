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

namespace UserInterface.Views
{
    public class BackgroundServiceSettings : UserControl
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;

        public BackgroundServiceSettings()
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
               
                
                
                await Task.Delay(1000);
            }
        }

        public void Start(object sender, RoutedEventArgs args)
        {
            //TODO: fix the path. This will depend on the deployment structure.
            System.Diagnostics.Process.Start("hass-worstation-service.exe");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
