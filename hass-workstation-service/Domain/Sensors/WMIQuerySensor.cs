using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.Versioning;
using System.Text;

namespace hass_workstation_service.Domain.Sensors
{

    [SupportedOSPlatform("windows")]
    public class WMIQuerySensor : AbstractSensor
    {
        public string Query { get; private set; }
        public string Scope { get; private set; }
        protected readonly ObjectQuery _objectQuery;
        protected readonly ManagementObjectSearcher _searcher;
        public WMIQuerySensor(MqttPublisher publisher, string query, int? updateInterval = null, string name = "WMIQuerySensor", Guid id = default, string scope = "") : base(publisher, name ?? "WMIQuerySensor", updateInterval ?? 10, id)
        {
            this.Query = query;
            this.Scope = scope;
            _objectQuery = new ObjectQuery(this.Query);
            ManagementScope managementscope;
            // if we have a custom scope, use that
            if (!string.IsNullOrWhiteSpace(scope))
            {
                managementscope = new ManagementScope(scope);
            }
            // otherwise, use the default
            else
            {
                managementscope = new ManagementScope(@"\\localhost\");
            }
            _searcher = new ManagementObjectSearcher(managementscope, _objectQuery);
        }
        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                NamePrefix = Publisher.NamePrefix,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{DiscoveryConfigModel.GetNameWithPrefix(Publisher.NamePrefix, this.ObjectId)}/state",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        public override string GetState()
        {
            using (ManagementObjectCollection collection = _searcher.Get())
            {
                foreach (ManagementObject mo in collection)
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        return property.Value.ToString();
                    }
                }
                return "";
            }
        }

    }
}
