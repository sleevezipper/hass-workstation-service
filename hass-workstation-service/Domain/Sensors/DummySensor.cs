using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using hass_workstation_service.Communication;

namespace hass_workstation_service.Domain.Sensors
{
    public class DummySensor : AbstractSensor
    {
        private readonly Random _random;
        public DummySensor(MqttPublisher publisher, int? updateInterval = null, string name = "Dummy", Guid id = default(Guid)) : base(publisher, name ?? "Dummy", updateInterval ?? 1, id)
        {
            this._random = new Random();
        }

        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        public override string GetState()
        {
            return _random.Next(0, 100).ToString();
        }
    }
}