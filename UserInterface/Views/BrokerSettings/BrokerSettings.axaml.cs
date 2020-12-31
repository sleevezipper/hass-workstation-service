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

namespace UserInterface.Views
{
    public class BrokerSettings : UserControl
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;
        private string _host { get; set; }
        private string _username { get; set; }
        private string _password { get; set; }
        public BrokerSettings()
        {
            DataContext = new BrokerSettingsViewModel();
            this.InitializeComponent();
                        // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<ServiceContractInterfaces>("client1", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<ServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<ServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("client1");

        }
        public void Ping(object sender, RoutedEventArgs args) {
            var result = this.client.InvokeAsync(x => x.Ping("ping")).Result;
        }

        public void Configure(object sender, RoutedEventArgs args)
        {
            var model = (BrokerSettingsViewModel)this.DataContext;
            var result = this.client.InvokeAsync(x => x.WriteMqttBrokerSettings(model.Host, model.Username, model.Password));
        }




        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
