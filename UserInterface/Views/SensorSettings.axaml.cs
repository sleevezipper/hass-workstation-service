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
    public class SensorSettings : UserControl
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;

        public SensorSettings()
        {
            this.InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<ServiceContractInterfaces>("sensors", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<ServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<ServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("sensors");


            DataContext = new BrokerSettingsViewModel();

        }
        public void Ping(object sender, RoutedEventArgs args) {
            var result = this.client.InvokeAsync(x => x.Ping("ping")).Result;
        }

        public void Configure(object sender, RoutedEventArgs args)
        {
            var model = (BrokerSettingsViewModel)this.DataContext;
            var result = this.client.InvokeAsync(x => x.WriteMqttBrokerSettingsAsync(new MqttSettings() { Host = model.Host, Username = model.Username, Password = model.Password }));
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
