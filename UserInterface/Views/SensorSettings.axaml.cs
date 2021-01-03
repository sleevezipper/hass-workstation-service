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
    public class SensorSettings : UserControl
    {
        private readonly IIpcClient<ServiceContractInterfaces> client;
        private DataGrid _dataGrid { get; set; }
        private bool sensorsNeedToRefresh { get; set; }

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


            DataContext = new SensorSettingsViewModel();
            GetConfiguredSensors();

            this._dataGrid = this.FindControl<DataGrid>("Grid");
        }


        public async void GetConfiguredSensors()
        {
            sensorsNeedToRefresh = false;
            List<ConfiguredSensorModel> status = await this.client.InvokeAsync(x => x.GetConfiguredSensors());

            ((SensorSettingsViewModel)this.DataContext).ConfiguredSensors = status.Select(s => new SensorViewModel() { Name = s.Name, Type = s.Type, Value = s.Value, Id = s.Id , UpdateInterval = s.UpdateInterval, UnitOfMeasurement = s.UnitOfMeasurement}).ToList();
            while (!sensorsNeedToRefresh)
            {
                await Task.Delay(1000);
                List<ConfiguredSensorModel> statusUpdated = await this.client.InvokeAsync(x => x.GetConfiguredSensors());
                var configuredSensors = ((SensorSettingsViewModel)this.DataContext).ConfiguredSensors;
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
        public void Delete(object sender, RoutedEventArgs args)
        {
            var item = ((SensorViewModel)this._dataGrid.SelectedItem);
            this.client.InvokeAsync(x => x.RemoveSensorById(item.Id));
            ((SensorSettingsViewModel)this.DataContext).ConfiguredSensors.Remove(item);
            this._dataGrid.SelectedIndex = -1;
            ((SensorSettingsViewModel)this.DataContext).TriggerUpdate();
        }

        public async void AddSensor(object sender, RoutedEventArgs args)
        {
            AddSensorDialog dialog = new AddSensorDialog();
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await dialog.ShowDialog(desktop.MainWindow);
                sensorsNeedToRefresh = true;
                GetConfiguredSensors();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


    }
}
