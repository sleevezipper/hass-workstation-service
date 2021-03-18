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
        private readonly IIpcClient<ServiceContractInterfaces> _client;

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
            this._client = clientFactory.CreateClient("broker");

            DataContext = new BackgroundServiceSettingsViewModel();
            Ping();
        }

        public async void Ping()
        {
            while (true)
            {
                if (DataContext is not BackgroundServiceSettingsViewModel viewModel)
                    throw new System.Exception("Wrong viewmodel class!");

                try
                {
                    var result = await this._client.InvokeAsync(x => x.Ping("ping"));

                    if (result == "pong")
                        viewModel.UpdateStatus(true, "All good");
                    else
                        viewModel.UpdateStatus(false, "Not running");
                }
                catch (System.Exception)
                {
                    viewModel.UpdateStatus(false, "Not running");
                }

                var autostartresult = await this._client.InvokeAsync(x => x.IsAutoStartEnabled());
                viewModel.UpdateAutostartStatus(autostartresult);

                await Task.Delay(1000);
            }
        }

        public void Start(object sender, RoutedEventArgs args)
        {
            //TODO: fix the path. This will depend on the deployment structure.
            System.Diagnostics.Process.Start("hass-worstation-service.exe");
        }

        public void EnableAutostart(object sender, RoutedEventArgs args)
        {
            this._client.InvokeAsync(x => x.EnableAutostart(true));
        }

        public void DisableAutostart(object sender, RoutedEventArgs args)
        {
            this._client.InvokeAsync(x => x.EnableAutostart(false));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}