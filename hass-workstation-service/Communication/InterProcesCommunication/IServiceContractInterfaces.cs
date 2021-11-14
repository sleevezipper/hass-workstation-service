using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hass_workstation_service.Communication.NamedPipe
{
    public interface IServiceContractInterfaces
    {
        Task<MqttSettings> GetMqttBrokerSettings();
        Task<GeneralSettings> GetGeneralSettings();
        void WriteGeneralSettings(GeneralSettings settings);
        public string Ping(string str);
        void WriteMqttBrokerSettingsAsync(MqttSettings settings);
        MqqtClientStatus GetMqqtClientStatus();
        void EnableAutostart(bool enable);
        bool IsAutoStartEnabled();
        Task<ConfiguredSensorModel> GetConfiguredSensor(Guid id);
        Task<List<ConfiguredSensorModel>> GetConfiguredSensors();
        void AddSensor(AvailableSensors sensorType, string json);
        void RemoveSensorById(Guid id);
        void UpdateSensorById(Guid id, string json);
        ConfiguredCommandModel GetConfiguredCommand(Guid id);
        List<ConfiguredCommandModel> GetConfiguredCommands();
        void AddCommand(AvailableCommands commandType, string json);
        void RemoveCommandById(Guid id);
        void UpdateCommandById(Guid id, string json);
        string GetCurrentVersion();
    }
}