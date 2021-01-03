using System;

namespace hass_workstation_service.Data
{
    public class ConfiguredSensor
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public int? UpdateInterval { get; set; }
    }
}