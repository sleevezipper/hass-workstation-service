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
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using hass_workstation_service.Data;

namespace UserInterface.Views
{
    public class GeneralSettingsView : UserControl
    {
        private readonly IIpcClient<IServiceContractInterfaces> client;

        public GeneralSettingsView()
        {
            this.InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<IServiceContractInterfaces>("general", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<IServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<IServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("general");


            DataContext = new GeneralSettingsViewModel();
            GetSettings();

        }

        public void Configure(object sender, RoutedEventArgs args)
        {
            var model = (GeneralSettingsViewModel)this.DataContext;
            ICollection<ValidationResult> results;
            if (model.IsValid(model, out results))
            {
                var result = this.client.InvokeAsync(x => x.WriteGeneralSettings(new GeneralSettings() { NamePrefix = model.NamePrefix }));
            }
        }

        public async void GetSettings()
        {
            GeneralSettings settings = await this.client.InvokeAsync(x => x.GetGeneralSettings());
            ((GeneralSettingsViewModel)this.DataContext).Update(settings);
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
