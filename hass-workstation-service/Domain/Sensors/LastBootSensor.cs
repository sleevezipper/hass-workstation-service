using hass_workstation_service.Communication;
using System;
using System.Runtime.InteropServices;

namespace hass_workstation_service.Domain.Sensors
{
    public class LastBootSensor : AbstractSensor
    {
        
        public LastBootSensor(MqttPublisher publisher, int? updateInterval = 10, string name = "LastBoot", Guid id = default) : base(publisher, name ?? "LastBoot", updateInterval ?? 10, id)
        {
        

        }

        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.ObjectId}/state",
                Icon = "mdi:clock-time-three-outline",
                Device_class = "timestamp"
            });
        }

        public override string GetState()
        {
            return (DateTime.Now - TimeSpan.FromMilliseconds(GetTickCount64())).ToString("s");
        }

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();
    }
}
