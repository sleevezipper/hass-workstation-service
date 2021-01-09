using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Text;

namespace hass_workstation_service.Domain.Sensors
{
    public class CurrentClockSpeedSensor : WMIQuerySensor
    {
        public CurrentClockSpeedSensor(MqttPublisher publisher, int? updateInterval = null, string name = "CurrentClockSpeed", Guid id = default(Guid)) : base(publisher, "SELECT CurrentClockSpeed FROM Win32_Processor", updateInterval ?? 10, name ?? "CurrentClockSpeed", id) { }

        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:speedometer",
                Unit_of_measurement = "MHz"
            });
        }
    }
}
