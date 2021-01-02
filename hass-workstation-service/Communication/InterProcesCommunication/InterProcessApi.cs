using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.NamedPipe;
using hass_workstation_service.Communication.Util;
using hass_workstation_service.Data;
using hass_workstation_service.Domain.Sensors;
using Serilog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hass_workstation_service.Communication.InterProcesCommunication
{
    public class InterProcessApi : ServiceContractInterfaces
    {
        private readonly MqttPublisher _publisher;
        private readonly IConfigurationService _configurationService;

        public InterProcessApi(MqttPublisher publisher, IConfigurationService configurationService)
        {
            _publisher = publisher;
            _configurationService = configurationService;
        }

        public MqqtClientStatus GetMqqtClientStatus()
        {
            return this._publisher.GetStatus();
        }

        public Task<MqttSettings> GetMqttBrokerSettings()
        {
            return this._configurationService.GetMqttBrokerSettings();
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

        public void WriteMqttBrokerSettingsAsync(MqttSettings settings)
        {
            this._configurationService.WriteMqttBrokerSettingsAsync(settings);
        }

        public void EnableAutostart(bool enable)
        {
            this._configurationService.EnableAutoStart(enable);
        }

        public bool IsAutoStartEnabled()
        {
            return this._configurationService.IsAutoStartEnabled();
        }

        public List<ConfiguredSensorModel> GetConfiguredSensors()
        {
            return this._configurationService.ConfiguredSensors.Select(s => new ConfiguredSensorModel() { Name = s.Name, Type = s.GetType().Name, Value = s.PreviousPublishedState, Id = s.Id }).ToList();
        }

        public void RemoveSensorById(Guid id)
        {
            this._configurationService.DeleteConfiguredSensor(id);
        }

        public void AddSensor(AvailableSensors sensorType, string json)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new DynamicJsonConverter() }
            };
            dynamic model = JsonSerializer.Deserialize<dynamic>(json, serializerOptions);

            AbstractSensor sensorToCreate = null;
            switch (sensorType)
            {
                case AvailableSensors.UserNotificationStateSensor:
                    sensorToCreate = new UserNotificationStateSensor(this._publisher, model.Name);
                    break;
                case AvailableSensors.DummySensor:
                    sensorToCreate = new DummySensor(this._publisher, model.Name);
                    break;
                default:
                    Log.Logger.Error("Unknown sensortype");
                    break;
            }
            if (sensorToCreate != null)
            {
                this._configurationService.AddConfiguredSensor(sensorToCreate);
            }
        }
    }
}
