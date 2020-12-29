using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using hass_workstation_service.Communication;

namespace hass_workstation_service.Domain.Sensors
{
    public class DummySensor : AbstractSensor
    {
        private readonly Random _random;
        public DummySensor(MqttPublisher publisher, string name = "Dummy")
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Publisher = publisher;
            this._random = new Random();
        }

        public DummySensor(MqttPublisher publisher, string name, Guid id)
        {
            this.Id = id;
            this.Name = name;
            this.Publisher = publisher;
            this._random = new Random();
        }
        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{this.Name}/state"
            });
        }

        public override string GetState()
        {
            return _random.Next(0, 100).ToString();
        }
    }
}