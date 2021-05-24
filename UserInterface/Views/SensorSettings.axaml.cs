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
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;

namespace UserInterface.Views
{
    public class SensorSettings : UserControl
    {
        private readonly IIpcClient<IServiceContractInterfaces> _client;
        private readonly DataGrid _dataGrid;
        private bool _sensorsNeedToRefresh;

        public SensorSettings()
        {
            InitializeComponent();
            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<IServiceContractInterfaces>("sensors", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<IServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<IServiceContractInterfaces>>();

            // create client
            _client = clientFactory.CreateClient("sensors");
            _dataGrid = this.FindControl<DataGrid>("Grid");

            DataContext = new SensorSettingsViewModel();
            GetConfiguredSensors();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void GetConfiguredSensors()
        {
            _sensorsNeedToRefresh = false;
            List<ConfiguredSensorModel> status = await _client.InvokeAsync(x => x.GetConfiguredSensors());

            ((SensorSettingsViewModel)DataContext).ConfiguredSensors = status.Select(s =>
                new SensorViewModel()
                {
                    Name = s.Name,
                    Type = s.Type,
                    Value = s.Value,
                    Id = s.Id,
                    UpdateInterval = s.UpdateInterval,
                    UnitOfMeasurement = s.UnitOfMeasurement
                }).ToList();

            while (!_sensorsNeedToRefresh)
            {
                await Task.Delay(1000);
                List<ConfiguredSensorModel> statusUpdated = await _client.InvokeAsync(x => x.GetConfiguredSensors());
                var configuredSensors = ((SensorSettingsViewModel)DataContext).ConfiguredSensors;
                // this is a workaround for the list showing before it has been completely loaded in the service
                if (statusUpdated.Count != configuredSensors.Count)
                {
                    _sensorsNeedToRefresh = true;
                    GetConfiguredSensors();
                }
                statusUpdated.ForEach(s =>
                {
                    var configuredSensor = configuredSensors.FirstOrDefault(cs => cs.Id == s.Id);
                    if (configuredSensor != null)
                    {
                        configuredSensor.Value = s.Value;

                        configuredSensors.FirstOrDefault(cs => cs.Id == s.Id).Value = s.Value;
                    }
                });
            }
        }

        public async void AddSensor(object sender, RoutedEventArgs args)
        {
            var dialog = new AddSensorDialog();
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                _sensorsNeedToRefresh = true;
                GetConfiguredSensors();
            }
        }

        public async void EditSensor(object sender, RoutedEventArgs args)
        {
            if (_dataGrid.SelectedItem is not SensorViewModel item)
                return;

            var dialog = new AddSensorDialog(item.Id);
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                _sensorsNeedToRefresh = true;
                GetConfiguredSensors();
            }
        }

        public void DeleteSensor(object sender, RoutedEventArgs args)
        {
            if (_dataGrid.SelectedItem is not SensorViewModel item)
                return;

            _client.InvokeAsync(x => x.RemoveSensorById(item.Id));

            if (DataContext is not SensorSettingsViewModel viewModel)
                return;

            viewModel.ConfiguredSensors.Remove(item);
            _dataGrid.SelectedIndex = -1;
            viewModel.TriggerUpdate();
        }
    }
}