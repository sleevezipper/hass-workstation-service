using hass_workstation_service.Domain.Sensors;
using System;

namespace hass_workstation_service.Data
{
    public class ConfiguredCommand
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
    }
}