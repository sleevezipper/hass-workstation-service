using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;
using hass_workstation_service.Communication;
using hass_workstation_service.Communication.NamedPipe;
using hass_workstation_service.Domain.Sensors;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Serilog;

namespace hass_workstation_service.Data
{
    public class ConfigurationService : ServiceContractInterfaces, IConfigurationService
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
            IsolatedStorageFileStream stream = this._fileStorage.OpenFile("configured-sensors.json", FileMode.OpenOrCreate);
            Log.Logger.Information($"reading configured sensors from: {stream.Name}");
            List<ConfiguredSensor> sensors = new List<ConfiguredSensor>();
            if (stream.Length > 0)
            {
                sensors = await JsonSerializer.DeserializeAsync<List<ConfiguredSensor>>(stream);
            }
            stream.Close();
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

        public async Task<IMqttClientOptions> ReadMqttSettings()
        {
            IsolatedStorageFileStream stream = this._fileStorage.OpenFile("mqttbroker.json", FileMode.OpenOrCreate);
            Log.Logger.Information($"reading configured mqttbroker from: {stream.Name}");
            ConfiguredMqttBroker configuredBroker = null;
            if (stream.Length > 0)
            {
                configuredBroker = await JsonSerializer.DeserializeAsync<ConfiguredMqttBroker>(stream);
            }
            stream.Close();
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

        public async void WriteSettings()
        {
            IsolatedStorageFileStream stream = this._fileStorage.OpenFile("configured-sensors.json", FileMode.OpenOrCreate);
            Log.Logger.Information($"writing configured sensors to: {stream.Name}");
            List<ConfiguredSensor> configuredSensorsToSave = new List<ConfiguredSensor>();

            foreach (AbstractSensor sensor in this.ConfiguredSensors)
            {
                configuredSensorsToSave.Add(new ConfiguredSensor() { Id = sensor.Id, Name = sensor.Name, Type = sensor.GetType().Name });
            }

            await JsonSerializer.SerializeAsync(stream, configuredSensorsToSave);
            stream.Close();
        }

        public void AddConfiguredSensor(AbstractSensor sensor)
        {
            this.ConfiguredSensors.Add(sensor);
            WriteSettings();
        }

        public void AddConfiguredSensors(List<AbstractSensor> sensors)
        {
            sensors.ForEach((sensor) => this.ConfiguredSensors.Add(sensor));
            WriteSettings();
        }

        public async void WriteMqttBrokerSettings(string host, string username, string password)
        {
            IsolatedStorageFileStream stream = this._fileStorage.OpenFile("mqttbroker.json", FileMode.OpenOrCreate);
            Log.Logger.Information($"writing configured mqttbroker to: {stream.Name}");
            ConfiguredMqttBroker configuredBroker = new ConfiguredMqttBroker()
            {
                Host = host,
                Username = username,
                Password = password
            };

            await JsonSerializer.SerializeAsync(stream, configuredBroker);
            stream.Close();

            this.MqqtConfigChangedHandler.Invoke(await this.ReadMqttSettings());
        }

        

        /// <summary>
        /// You can use this to check if the application responds.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Ping(string str)
        {
            if (str == "ping")
            {
                return "pong";
            }
            return "what?";
        }
    }
}