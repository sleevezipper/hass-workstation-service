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
    public class BrokerSettings : UserControl
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;

        public BrokerSettings()
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


            DataContext = new BrokerSettingsViewModel();
            GetSettings();
            GetStatus();

        }
        public void Ping(object sender, RoutedEventArgs args) {
            var result = this.client.InvokeAsync(x => x.Ping("ping")).Result;
        }

        public void Configure(object sender, RoutedEventArgs args)
        {
            var model = (BrokerSettingsViewModel)this.DataContext;
            var result = this.client.InvokeAsync(x => x.WriteMqttBrokerSettingsAsync(new MqttSettings() { Host = model.Host, Username = model.Username, Password = model.Password }));
        }

        public async void GetSettings()
        {
            MqttSettings settings = await this.client.InvokeAsync(x => x.GetMqttBrokerSettings());
            ((BrokerSettingsViewModel)this.DataContext).Update(settings);
        }

        public async void GetStatus()
        {
            while (true)
            {
                try
                {
                    MqqtClientStatus status = await this.client.InvokeAsync(x => x.GetMqqtClientStatus());

                    ((BrokerSettingsViewModel)this.DataContext).UpdateStatus(status);
                    await Task.Delay(1000);
                }
                catch (System.Exception)
                {
                    ((BrokerSettingsViewModel)this.DataContext).UpdateStatus(new MqqtClientStatus() {IsConnected = false, Message = "Unable to get connectionstatus" });
                }

            }
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
