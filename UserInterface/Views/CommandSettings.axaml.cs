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
        private readonly IIpcClient<IServiceContractInterfaces> _client;
        private readonly DataGrid _dataGrid;
        private bool _sensorsNeedToRefresh;

        public CommandSettings()
        {
            this.InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<IServiceContractInterfaces>("commands", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<IServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<IServiceContractInterfaces>>();

            // create client
            this._client = clientFactory.CreateClient("commands");


            DataContext = new CommandSettingsViewModel();
            GetConfiguredCommands();

            this._dataGrid = this.FindControl<DataGrid>("Grid");
        }

        public async void GetConfiguredCommands()
        {
            _sensorsNeedToRefresh = false;
            List<ConfiguredCommandModel> status = await this._client.InvokeAsync(x => x.GetConfiguredCommands());

            ((CommandSettingsViewModel)this.DataContext).ConfiguredCommands = status.Select(s =>
                new CommandViewModel()
                {
                    Name = s.Name,
                    Type = s.Type,
                    Id = s.Id
                }).ToList();
        }

        public async void AddCommand(object sender, RoutedEventArgs args)
        {
            var dialog = new AddCommandDialog();
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                _sensorsNeedToRefresh = true;
                GetConfiguredCommands();
            }
        }

        public void EditCommand(object sender, RoutedEventArgs args)
        {

        }

        public void DeleteCommand(object sender, RoutedEventArgs args)
        {
            if (_dataGrid.SelectedItem is not CommandViewModel item)
                return;

            this._client.InvokeAsync(x => x.RemoveCommandById(item.Id));

            if (DataContext is not CommandSettingsViewModel viewModel)
                return;

            viewModel.ConfiguredCommands.Remove(item);
            _dataGrid.SelectedIndex = -1;
            viewModel.TriggerUpdate();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}