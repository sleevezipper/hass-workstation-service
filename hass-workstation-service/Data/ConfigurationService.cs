using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;
using hass_workstation_service.Communication;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.NamedPipe;
using hass_workstation_service.Domain.Sensors;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Serilog;

namespace hass_workstation_service.Data
{
    public class ConfigurationService : IConfigurationService
    {
        public ICollection<AbstractSensor> ConfiguredSensors { get; private set; }
        public Action<IMqttClientOptions> MqqtConfigChangedHandler { get; set; }

        private readonly IsolatedStorageFile _fileStorage;

        public ConfigurationService()
        {
            this._fileStorage = IsolatedStorageFile.GetUserStoreForApplication();

            ConfiguredSensors = new List<AbstractSensor>();
        }

        public async void ReadSensorSettings(MqttPublisher publisher)
        {
            List<ConfiguredSensor> sensors = new List<ConfiguredSensor>();
            using (var stream = this._fileStorage.OpenFile("configured-sensors.json", FileMode.OpenOrCreate))
            {
                Log.Logger.Information($"reading configured sensors from: {stream.Name}");
                if (stream.Length > 0)
                {
                    sensors = await JsonSerializer.DeserializeAsync<List<ConfiguredSensor>>(stream);
                }
            }

            foreach (ConfiguredSensor configuredSensor in sensors)
            {
                AbstractSensor sensor;
                switch (configuredSensor.Type)
                {
                    case "UserNotificationStateSensor":
                        sensor = new UserNotificationStateSensor(publisher, configuredSensor.Name, configuredSensor.Id);
                        break;
                    case "DummySensor":
                        sensor = new DummySensor(publisher, configuredSensor.Name, configuredSensor.Id);
                        break;
                    default:
                        throw new InvalidOperationException("unsupported sensor type in config");
                }
                this.ConfiguredSensors.Add(sensor);
            }
        }

        public async Task<IMqttClientOptions> GetMqttClientOptionsAsync()
        {
            ConfiguredMqttBroker configuredBroker = await ReadMqttSettingsAsync();
            if (configuredBroker != null && configuredBroker.Host != null)
            {

                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(configuredBroker.Host)
                    // .WithTls()
                    .WithCredentials(configuredBroker.Username, configuredBroker.Password.ToString())
                    .Build();
                return mqttClientOptions;
            }
            else
            {
                //for now we return defaults until we can actually configure this
                return new MqttClientOptionsBuilder()
                    .WithTcpServer("192.168.2.6")
                    // .WithTls()
                    .WithCredentials("tester", "tester")
                    .Build();
            }
        }

        /// <summary>
        /// Gets the saved broker settings from configfile. Null if not found.
        /// </summary>
        /// <returns></returns>
        public async Task<ConfiguredMqttBroker> ReadMqttSettingsAsync()
        {
            ConfiguredMqttBroker configuredBroker = null;
            using (IsolatedStorageFileStream stream = this._fileStorage.OpenFile("mqttbroker.json", FileMode.OpenOrCreate))
            {
                Log.Logger.Information($"reading configured mqttbroker from: {stream.Name}");
                if (stream.Length > 0)
                {
                    configuredBroker = await JsonSerializer.DeserializeAsync<ConfiguredMqttBroker>(stream);
                }
            }

            return configuredBroker;
        }

        public async void WriteSettingsAsync()
        {
            List<ConfiguredSensor> configuredSensorsToSave = new List<ConfiguredSensor>();
            using (IsolatedStorageFileStream stream = this._fileStorage.OpenFile("configured-sensors.json", FileMode.OpenOrCreate))
            {
                Log.Logger.Information($"writing configured sensors to: {stream.Name}");
                foreach (AbstractSensor sensor in this.ConfiguredSensors)
                {
                    configuredSensorsToSave.Add(new ConfiguredSensor() { Id = sensor.Id, Name = sensor.Name, Type = sensor.GetType().Name });
                }

                await JsonSerializer.SerializeAsync(stream, configuredSensorsToSave);
            }
        }

        public void AddConfiguredSensor(AbstractSensor sensor)
        {
            this.ConfiguredSensors.Add(sensor);
            WriteSettingsAsync();
        }

        public void AddConfiguredSensors(List<AbstractSensor> sensors)
        {
            sensors.ForEach((sensor) => this.ConfiguredSensors.Add(sensor));
            WriteSettingsAsync();
        }

        /// <summary>
        /// Writes provided settings to the config file and invokes a reconfigure to the current mqqtClient
        /// </summary>
        /// <param name="settings"></param>
        public async void WriteMqttBrokerSettingsAsync(MqttSettings settings)
        {
            using (IsolatedStorageFileStream stream = this._fileStorage.OpenFile("mqttbroker.json", FileMode.OpenOrCreate))
            {
                Log.Logger.Information($"writing configured mqttbroker to: {stream.Name}");

                ConfiguredMqttBroker configuredBroker = new ConfiguredMqttBroker()
                {
                    Host = settings.Host,
                    Username = settings.Username,
                    Password = settings.Password
                };

                await JsonSerializer.SerializeAsync(stream, configuredBroker);
            }

            this.MqqtConfigChangedHandler.Invoke(await this.GetMqttClientOptionsAsync());
        }

        public async Task<MqttSettings> GetMqttBrokerSettings()
        {
            ConfiguredMqttBroker broker = await ReadMqttSettingsAsync();
            return new MqttSettings
            {
                Host = broker?.Host,
                Username = broker?.Username,
                Password = broker?.Password
            };
        }

        /// <summary>
        /// Enable or disable autostarting the background service. It does this by adding the application shortcut (appref-ms) to the registry run key for the current user
        /// </summary>
        /// <param name="enable"></param>
        public void EnableAutoStart(bool enable)
        {
            if (enable)
            {
                Log.Logger.Information("configuring autostart");
                // The path to the key where Windows looks for startup applications
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                //Path to launch shortcut
                string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\hass-workstation-service\hass-workstation-service.appref-ms";

                rkApp.SetValue("hass-workstation-service", startPath);
                rkApp.Close();
            }
            else
            {
                Log.Logger.Information("removing autostart");
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                rkApp.DeleteValue("hass-workstation-service");
                rkApp.Close();
            }
        }

        public bool IsAutoStartEnabled()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            return rkApp.GetValue("hass-workstation-service") != null;
        }
    }
}