using System;
using System.Security;

namespace hass_workstation_service.Data
{
    public class ConfiguredMqttBroker
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}