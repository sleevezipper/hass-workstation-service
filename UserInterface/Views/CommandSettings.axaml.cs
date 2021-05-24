using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using hass_workstation_service.Communication.NamedPipe;
using JKang.IpcServiceFramework.Client;
using Avalonia.Interactivity;
using System.Reactive.Linq;
using UserInterface.ViewModels;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace UserInterface.Views
{
    public class CommandSettings : UserControl
    {
        private readonly IIpcClient<IServiceContractInterfaces> _client;
        private readonly DataGrid _dataGrid;
        private bool _commandsNeedToRefresh;

        public CommandSettings()
        {
            InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<IServiceContractInterfaces>("commands", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<IServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<IServiceContractInterfaces>>();

            // create client
            _client = clientFactory.CreateClient("commands");
            _dataGrid = this.FindControl<DataGrid>("Grid");

            DataContext = new CommandSettingsViewModel();
            GetConfiguredCommands();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void GetConfiguredCommands()
        {
            List<ConfiguredCommandModel> status = await _client.InvokeAsync(x => x.GetConfiguredCommands());

            ((CommandSettingsViewModel)DataContext).ConfiguredCommands = status.Select(s =>
                new CommandViewModel()
                {
                    Name = s.Name,
                    Type = s.Type,
                    Id = s.Id
                }).ToList();

            if (_commandsNeedToRefresh)
            {
                await Task.Delay(1000);
                GetConfiguredCommands();
                _commandsNeedToRefresh = false;
            }
        }

        public async void AddCommand(object sender, RoutedEventArgs args)
        {
            var dialog = new AddCommandDialog();
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                GetConfiguredCommands();
            }
        }

        public async void EditCommand(object sender, RoutedEventArgs args)
        {
            if (_dataGrid.SelectedItem is not CommandViewModel item)
                return;

            var dialog = new AddCommandDialog(item.Id);
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                _commandsNeedToRefresh = true;
                GetConfiguredCommands();
            }
        }

        public void DeleteCommand(object sender, RoutedEventArgs args)
        {
            if (_dataGrid.SelectedItem is not CommandViewModel item)
                return;

            _client.InvokeAsync(x => x.RemoveCommandById(item.Id));

            if (DataContext is not CommandSettingsViewModel viewModel)
                return;

            viewModel.ConfiguredCommands.Remove(item);
            _dataGrid.SelectedIndex = -1;
            viewModel.TriggerUpdate();
        }
    }
}