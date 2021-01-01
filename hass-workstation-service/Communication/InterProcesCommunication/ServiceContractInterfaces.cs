using hass_workstation_service.Communication.InterProcesCommunication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Communication.NamedPipe
{
    public interface ServiceContractInterfaces
    {
        Task<MqttSettings> GetMqttBrokerSettings();
        public string Ping(string str);
        void WriteMqttBrokerSettingsAsync(MqttSettings settings);
        MqqtClientStatus GetMqqtClientStatus();
        void EnableAutostart(bool enable);
        bool IsAutoStartEnabled();
    }
}
