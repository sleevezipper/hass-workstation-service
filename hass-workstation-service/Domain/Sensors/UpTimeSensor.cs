using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HWND = System.IntPtr;

namespace hass_workstation_service.Domain.Sensors
{
    public class UpTimeSensor : AbstractSensor
    {
        
        public UpTimeSensor(MqttPublisher publisher, int? updateInterval = 10, string name = "UpTime", Guid id = default) : base(publisher, name ?? "UpTime", updateInterval ?? 10, id)
        {
        

        }

        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:clock-time-three-outline",
            });
        }

        public override string GetState()
        {

            return (GetTickCount64() / 1000).ToString(); //return in seconds
        }

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();
    }
}
