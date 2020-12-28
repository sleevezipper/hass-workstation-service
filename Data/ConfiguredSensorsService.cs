using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using hass_desktop_service.Communication;
using hass_desktop_service.Domain.Sensors;
using Microsoft.Extensions.Configuration;

namespace hass_desktop_service.Data
{
    public class ConfiguredSensorsService
    {
        public ICollection<AbstractSensor> ConfiguredSensors { get; private set; }
        public IConfiguration Configuration { get; private set; }
        private readonly MqttPublisher _publisher;
        private readonly IsolatedStorageFile _fileStorage;

        public ConfiguredSensorsService(MqttPublisher publisher)
        {
            this._fileStorage = IsolatedStorageFile.GetUserStoreForApplication();

            ConfiguredSensors = new List<AbstractSensor>();
            _publisher = publisher;
            ReadSettings();
        }

        public async void ReadSettings()
        {
            IsolatedStorageFileStream stream = this._fileStorage.OpenFile("configured-sensors.json", FileMode.OpenOrCreate);
            List<ConfiguredSensor> sensors = await JsonSerializer.DeserializeAsync<List<ConfiguredSensor>>(stream);

            foreach (ConfiguredSensor configuredSensor in sensors)
            {
                AbstractSensor sensor;
                #pragma warning disable IDE0066
                switch (configuredSensor.Type)
                {
                    case "UserNotificationStateSensor":
                        sensor = new UserNotificationStateSensor(_publisher, configuredSensor.Name, configuredSensor.Id);
                        break;
                    default:
                        throw new InvalidOperationException("unsupported sensor type in config");
                }
                this.ConfiguredSensors.Add(sensor);
            }
        }

        public void AddConfiguredSensor(AbstractSensor sensor)
        {
            
        }
    }
}