using hass_workstation_service.Communication;
using hass_workstation_service.Domain.Sensors;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace hass_workstation_service.Data
{
    public interface IConfigurationService
    {
        ICollection<AbstractSensor> ConfiguredSensors { get; }
        Action<IMqttClientOptions> MqqtConfigChangedHandler { get; set; }

        void AddConfiguredSensor(AbstractSensor sensor);
        void AddConfiguredSensors(List<AbstractSensor> sensors);
        string Ping(string str);
        Task<IMqttClientOptions> ReadMqttSettings();
        void ReadSensorSettings(MqttPublisher publisher);
        void WriteMqttBrokerSettings(string host, string username, string password);
        void WriteSettings();
    }
}