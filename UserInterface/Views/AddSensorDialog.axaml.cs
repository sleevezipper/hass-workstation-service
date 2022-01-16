using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.NamedPipe;
using JKang.IpcServiceFramework.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.Json;
using UserInterface.Util;
using UserInterface.ViewModels;

namespace UserInterface.Views
{
    public class AddSensorDialog : Window
    {
        private readonly IIpcClient<IServiceContractInterfaces> _client;
        public ComboBox ComboBox { get; set; }
        public ComboBox DetectionModecomboBox { get; set; }
        public Guid SensorId { get; }

        public AddSensorDialog(Guid sensorId) : this()
        {
            SensorId = sensorId;
            GetSensorInfo(SensorId);
            Title = "Edit sensor";
        }

        public AddSensorDialog()
        {
            InitializeComponent();
            DataContext = new AddSensorViewModel();
            ComboBox = this.FindControl<ComboBox>("ComboBox");
            ComboBox.Items = Enum.GetValues(typeof(AvailableSensors)).Cast<AvailableSensors>().OrderBy(v => v.ToString());
            ComboBox.SelectedIndex = 0;

            // register IPC clients
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddNamedPipeIpcClient<IServiceContractInterfaces>("addsensor", pipeName: "pipeinternal")
                .BuildServiceProvider();

            // resolve IPC client factory
            IIpcClientFactory<IServiceContractInterfaces> clientFactory = serviceProvider
                .GetRequiredService<IIpcClientFactory<IServiceContractInterfaces>>();

            // create client
            _client = clientFactory.CreateClient("addsensor");
            Title = "Add sensor";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void GetSensorInfo(Guid sensorId)
        {
            ConfiguredSensorModel sensor = await _client.InvokeAsync(x => x.GetConfiguredSensor(sensorId));

            ComboBox.SelectedItem = sensor.Type;
            FillDefaultValues();
            ComboBox.IsEnabled = false;
            var item = (AddSensorViewModel)DataContext;
            item.SelectedType = sensor.Type;
            item.Name = sensor.Name;
            item.UpdateInterval = sensor.UpdateInterval;
            item.Query = sensor.Query;
            item.Scope = sensor.Scope;
            item.WindowName = sensor.WindowName;

            Title = $"Edit {sensor.Name}";
        }

        public async void Save(object sender, RoutedEventArgs args)
        {
            var item = (AddSensorViewModel)DataContext;
            dynamic model = new { item.Name, item.Query, item.UpdateInterval, item.WindowName, item.Scope };
            string json = JsonSerializer.Serialize(model);
            if (SensorId == Guid.Empty)
                await _client.InvokeAsync(x => x.AddSensor(item.SelectedType, json));
            else
                await _client.InvokeAsync(x => x.UpdateSensorById(SensorId, json));

            Close();
        }

        public void ComboBoxClosed(object sender, SelectionChangedEventArgs args)
        {
            FillDefaultValues();
        }

        private void FillDefaultValues()
        {
            var item = (AddSensorViewModel)DataContext;
            switch (ComboBox.SelectedItem)
            {
                case AvailableSensors.UserNotificationStateSensor:
                    item.Description = "This sensor watches the UserNotificationState. This is normally used in applications to determine if it is appropriate to send a notification but we can use it to expose this state. \n ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#usernotificationstate";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.DummySensor:
                    item.Description = "This sensor spits out a random number every second. Useful for testing, maybe you'll find some other use for it.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#dummysensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 1;
                    break;

                case AvailableSensors.CPULoadSensor:
                    item.Description = "This sensor checks the current CPU load. It averages the load on all logical cores every second and rounds the output to two decimals.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#cpuloadsensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.CurrentClockSpeedSensor:
                    item.Description = "This sensor returns the BIOS configured baseclock for the processor.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#currentclockspeedsensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 3600;
                    break;

                case AvailableSensors.WMIQuerySensor:
                    item.Description = "This advanced sensor executes a user defined WMI query and exposes the result. The query should return a single value.";
                    item.MoreInfoLink = "https://github.com/sleevezipperhass-workstation-service/blob/master/documentation/WMIQuery.md#wmiquerysensor";
                    item.ShowQueryInput = true;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 10;
                    break;

                case AvailableSensors.MemoryUsageSensor:
                    item.Description = "This sensor calculates the percentage of used memory.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#memoryusagesensorsensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 10;
                    break;

                case AvailableSensors.ActiveWindowSensor:
                    item.Description = "This sensor exposes the name of the currently active window.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#activewindowsensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.WebcamActiveSensor:
                    item.Description = "This sensor shows if the webcam is currently being used.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#webcamactivesensor";
                    item.ShowQueryInput = false;
                    item.UpdateInterval = 10;
                    break;

                case AvailableSensors.WebcamProcessSensor:
                    item.Description = "This sensor shows which process is using the webcam.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#webcamprocesssensor";
                    item.ShowQueryInput = false;
                    item.UpdateInterval = 10;
                    break;

                case AvailableSensors.MicrophoneActiveSensor:
                    item.Description = "This sensor shows if the microphone is currently in use.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#microphoneactivesensor";
                    item.ShowQueryInput = false;
                    item.UpdateInterval = 10;
                    break;

                case AvailableSensors.MicrophoneProcessSensor:
                    item.Description = "This sensor shows which process is using the microphone.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#microphoneprocesssensor";
                    item.ShowQueryInput = false;
                    item.UpdateInterval = 10;
                    break;

                case AvailableSensors.NamedWindowSensor:
                    item.Description = "This sensor returns true if a window was found with the name you search for. ";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#namedwindowsensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = true;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.LastActiveSensor:
                    item.Description = "This sensor returns the date/time that the workstation was last active.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#lastactivesensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.LastBootSensor:
                    item.Description = "This sensor returns the date/time that Windows was last booted";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#lastbootsensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.SessionStateSensor:
                    item.Description = "This sensor returns the state of the Windows session.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#sessionstatesensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.CurrentVolumeSensor:
                    item.Description = "This sensor returns the volume of currently playing audio.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#currentvolumesensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.MasterVolumeSensor:
                    item.Description = "This sensor returns the master volume of the currently selected default audio device as a percentage value.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#mastervolumesensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.GPUTemperatureSensor:
                    item.Description = "This sensor returns the current temperature of the GPU in °C.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#gputemperaturesensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                case AvailableSensors.GPULoadSensor:
                    item.Description = "This sensor returns the current GPU load.";
                    item.MoreInfoLink = "https://github.com/sleevezipper/hass-workstation-service/blob/master/documentation/Sensors.md#gpuloadsensor";
                    item.ShowQueryInput = false;
                    item.ShowWindowNameInput = false;
                    item.UpdateInterval = 5;
                    break;

                default:
                    item.Description = null;
                    item.MoreInfoLink = null;
                    item.ShowQueryInput = false;
                    break;
            }
        }

        public void OpenInfo(object sender, RoutedEventArgs args)
        {
            var item = (AddSensorViewModel)DataContext;
            BrowserUtil.OpenBrowser(item.MoreInfoLink);
        }
    }
}
