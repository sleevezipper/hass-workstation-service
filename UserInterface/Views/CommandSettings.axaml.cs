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
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;

namespace UserInterface.Views
{
    public class CommandSettings : UserControl
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;
        private DataGrid _dataGrid { get; set; }
        private bool sensorsNeedToRefresh { get; set; }

        public CommandSettings()
        {
            this.InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<ServiceContractInterfaces>("commands", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<ServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<ServiceContractInterfaces>>();

            // create client
            this.client = clientFactory.CreateClient("commands");


            DataContext = new CommandSettingsViewModel();
            GetConfiguredCommands();

            this._dataGrid = this.FindControl<DataGrid>("Grid");
        }


        public async void GetConfiguredCommands()
        {
            sensorsNeedToRefresh = false;
            List<ConfiguredCommandModel> status = await this.client.InvokeAsync(x => x.GetConfiguredCommands());

            ((CommandSettingsViewModel)this.DataContext).ConfiguredCommands = status.Select(s => new CommandViewModel() { Name = s.Name, Type = s.Type, Id = s.Id}).ToList();

        }
        public void Delete(object sender, RoutedEventArgs args)
        {
            var item = ((CommandViewModel)this._dataGrid.SelectedItem);
            this.client.InvokeAsync(x => x.RemoveCommandById(item.Id));
            ((CommandSettingsViewModel)this.DataContext).ConfiguredCommands.Remove(item);
            this._dataGrid.SelectedIndex = -1;
            ((CommandSettingsViewModel)this.DataContext).TriggerUpdate();
        }

        public async void Add(object sender, RoutedEventArgs args)
        {
            AddCommandDialog dialog = new AddCommandDialog();
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                sensorsNeedToRefresh = true;
                GetConfiguredCommands();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
