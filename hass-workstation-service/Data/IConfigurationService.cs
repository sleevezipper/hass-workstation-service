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
        Action<IManagedMqttClientOptions> MqqtConfigChangedHandler { get; set; }
        ICollection<AbstractSensor> ConfiguredSensors { get; }
        ICollection<AbstractCommand> ConfiguredCommands { get; }

        void AddConfiguredSensor(AbstractSensor sensor);
        void AddConfiguredCommand(AbstractCommand command);
        void AddConfiguredSensors(List<AbstractSensor> sensors);
        void AddConfiguredCommands(List<AbstractCommand> commands);
        void DeleteConfiguredSensor(Guid id);
        void DeleteConfiguredCommand(Guid id);
        void UpdateConfiguredSensor(Guid id, AbstractSensor sensor);
        void UpdateConfiguredCommand(Guid id, AbstractCommand command);
        Task<IManagedMqttClientOptions> GetMqttClientOptionsAsync();
        void ReadSensorSettings(MqttPublisher publisher);
        void WriteMqttBrokerSettingsAsync(MqttSettings settings);
        void WriteSensorSettingsAsync();
        Task<MqttSettings> GetMqttBrokerSettings();
        void EnableAutoStart(bool enable);
        bool IsAutoStartEnabled();
        void WriteCommandSettingsAsync();
        void ReadCommandSettings(MqttPublisher publisher);
        Task<ICollection<AbstractSensor>> GetSensorsAfterLoadingAsync();
    }
}