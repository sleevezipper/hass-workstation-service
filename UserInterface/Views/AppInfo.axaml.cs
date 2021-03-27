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
        private readonly IIpcClient<IServiceContractInterfaces> client;

        public AppInfo()
        {
            this.InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<IServiceContractInterfaces>("info", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<IServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<IServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("info");




            DataContext = new InfoViewModel();
            UpdateVersion();
        }
        public async void UpdateVersion() {

            try
            {
                var result = await this.client.InvokeAsync(x => x.GetCurrentVersion());
                ((InfoViewModel)this.DataContext).UpdateServiceVersion(result);

            }
            catch (System.Exception)
            {

            }
        }

        public void GitHub(object sender, RoutedEventArgs args)
        {
            BrowserUtil.OpenBrowser("https://github.com/sleevezipper/hass-workstation-service");
        }

        public void Discord(object sender, RoutedEventArgs args)
        {
            BrowserUtil.OpenBrowser("https://discord.gg/VraYT2N3wd");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
