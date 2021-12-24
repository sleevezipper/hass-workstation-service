using System;
using System.Security;

namespace hass_workstation_service.Data
{
    public class ConfiguredMqttBroker
    {
        private string username;
        private string password;
        private int? port;
        
        private string rootCAPath;
        private string clientCertPath;

        public string Host { get; set; }
        public int Port { get => port ?? 1883; set => port = value; }
        public bool UseTLS { get; set; }

        // Before this option, Retains was the default, so let's keep that here to not break backwards compatibility
        public bool RetainLWT { get; set; } = true;

        public string RootCAPath { 
            get
            {
                if (rootCAPath!= null) return rootCAPath;
                return "";
            }
            set => rootCAPath = value; 
        }
        public string ClientCertPath {
            get
            {
                if (clientCertPath != null) return clientCertPath;
                return "";
            }
            set => clientCertPath = value;
        }

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