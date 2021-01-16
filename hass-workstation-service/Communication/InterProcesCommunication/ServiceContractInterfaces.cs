using hass_workstation_service.Communication.InterProcesCommunication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Communication.NamedPipe
{
    public interface ServiceContractInterfaces
    {
        Task<MqttSettings> GetMqttBrokerSettings();
        public string Ping(string str);
        void WriteMqttBrokerSettingsAsync(MqttSettings settings);
        MqqtClientStatus GetMqqtClientStatus();
        void EnableAutostart(bool enable);
        bool IsAutoStartEnabled();
        List<ConfiguredSensorModel> GetConfiguredSensors();
        void RemoveSensorById(Guid id);
        void AddSensor(AvailableSensors sensorType, string json);
        void RemoveCommandById(Guid id);
        List<ConfiguredCommandModel> GetConfiguredCommands();
        void AddCommand(AvailableCommands commandType, string json);
    }
}
