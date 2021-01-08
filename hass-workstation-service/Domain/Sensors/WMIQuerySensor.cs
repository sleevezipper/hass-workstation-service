using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace hass_workstation_service.Domain.Sensors
{
    public class WMIQuerySensor : AbstractSensor
    {
        public string Query { get; private set; }
        protected readonly ObjectQuery _objectQuery;
        protected readonly ManagementObjectSearcher _searcher;
        public WMIQuerySensor(MqttPublisher publisher, string query, int? updateInterval = null, string name = "WMIQuerySensor", Guid id = default(Guid)) : base(publisher, name ?? "WMIQuerySensor", updateInterval ?? 10, id)
        {
            this.Query = query;
            _objectQuery = new ObjectQuery(this.Query);
            _searcher = new ManagementObjectSearcher(query);
        }
        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
            });
        }

        public override string GetState()
        {
            ManagementObjectCollection collection = _searcher.Get();

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
