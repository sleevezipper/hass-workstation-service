using System;
using System.Security;

namespace hass_workstation_service.Data
{
    public class ConfiguredMqttBroker
    {
        private string username;
        private string password;
        public string Host { get; set; }

        public string Username
        {
            get
            {
                if (username != null) return username;
                
                return "";
            }
            set => username = value;
        }

        public string Password
        {
            get
            {
                if (password != null) return password;
                
                return "";
            }
            set => password = value;
        }
    }
}