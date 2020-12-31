using System;
using System.Collections.Generic;
using System.Text;

namespace hass_workstation_service.Communication.NamedPipe
{
    public interface ServiceContractInterfaces
    {
        public string Ping(string str);
        void WriteMqttBrokerSettings(string host, string username, string password);
    }
}
