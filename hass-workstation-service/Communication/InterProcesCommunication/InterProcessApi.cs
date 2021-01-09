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
            return this._configurationService.ConfiguredSensors.Select(s => new ConfiguredSensorModel() { Name = s.Name, Type = s.GetType().Name, Value = s.PreviousPublishedState, Id = s.Id, UpdateInterval = s.UpdateInterval, UnitOfMeasurement = s.GetAutoDiscoveryConfig().Unit_of_measurement }).ToList();
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
                    sensorToCreate = new UserNotificationStateSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.DummySensor:
                    sensorToCreate = new DummySensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.CurrentClockSpeedSensor:
                    sensorToCreate = new CurrentClockSpeedSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.CPULoadSensor:
                    sensorToCreate = new CPULoadSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.WMIQuerySensor:
                    sensorToCreate = new WMIQuerySensor(this._publisher, model.Query, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.MemoryUsageSensor:
                    sensorToCreate = new MemoryUsageSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.ActiveWindowSensor:
                    sensorToCreate = new ActiveWindowSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.WebcamActiveSensor:
                    sensorToCreate = new WebcamActiveSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.MicrophoneActiveSensor:
                    sensorToCreate = new MicrophoneActiveSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.NamedWindowSensor:
                    sensorToCreate = new NamedWindowSensor(this._publisher, model.WindowName, model.Name, (int)model.UpdateInterval);
                    break;
                case AvailableSensors.IdleTimeSensor:
                    sensorToCreate = new IdleTimeSensor(this._publisher,(int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.UpTimeSensor:
                    sensorToCreate = new UpTimeSensor(this._publisher, (int)model.UpdateInterval, model.Name);
                    break;
                case AvailableSensors.SessionStateSensor:
                    sensorToCreate = new SessionStateSensor(this._publisher, (int)model.UpdateInterval, model.Name);
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
