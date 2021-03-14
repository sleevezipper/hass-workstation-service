using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using System.Text;

namespace hass_workstation_service.Domain.Sensors
{

    [SupportedOSPlatform("windows")]
    public class CPULoadSensor : WMIQuerySensor
    {
        public CPULoadSensor(MqttPublisher publisher, int? updateInterval = null, string name = "CPULoadSensor", Guid id = default) : base(publisher, "SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor", updateInterval ?? 10, name ?? "CPULoadSensor", id)
        {

        }
        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:chart-areaspline",
                Unit_of_measurement = "%",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        [SupportedOSPlatform("windows")]
        public override string GetState()
        {
            using (ManagementObjectCollection collection = _searcher.Get())
            {
                List<int> processorLoadPercentages = new List<int>();
                foreach (ManagementObject mo in collection)
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        processorLoadPercentages.Add(int.Parse(property.Value.ToString()));
                    }
                }
                double average = processorLoadPercentages.Count > 0 ? processorLoadPercentages.Average() : 0.0;
                return average.ToString("#.##", CultureInfo.InvariantCulture);
            }

        }
    }
}
