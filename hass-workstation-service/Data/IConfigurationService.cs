using hass_workstation_service.Communication;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Domain.Commands;
using hass_workstation_service.Domain.Sensors;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace hass_workstation_service.Data
{
    public interface IConfigurationService
    {
        ICollection<AbstractSensor> ConfiguredSensors { get; }
        Action<IManagedMqttClientOptions> MqqtConfigChangedHandler { get; set; }
        ICollection<AbstractCommand> ConfiguredCommands { get; }

        void AddConfiguredCommand(AbstractCommand command);
        void AddConfiguredSensor(AbstractSensor sensor);
        void AddConfiguredSensors(List<AbstractSensor> sensors);
        Task<IManagedMqttClientOptions> GetMqttClientOptionsAsync();
        void ReadSensorSettings(MqttPublisher publisher);
        void WriteMqttBrokerSettingsAsync(MqttSettings settings);
        void WriteSensorSettingsAsync();
        Task<MqttSettings> GetMqttBrokerSettings();
        void EnableAutoStart(bool enable);
        bool IsAutoStartEnabled();
        void DeleteConfiguredSensor(Guid id);
        void DeleteConfiguredCommand(Guid id);
        void WriteCommandSettingsAsync();
        void ReadCommandSettings(MqttPublisher publisher);
    }
}