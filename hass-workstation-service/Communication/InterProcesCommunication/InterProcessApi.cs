using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.NamedPipe;
using hass_workstation_service.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Communication.InterProcesCommunication
{
    public class InterProcessApi : ServiceContractInterfaces
    {
        private readonly MqttPublisher _publisher;
        private readonly IConfigurationService _configurationService;

        public InterProcessApi(MqttPublisher publisher, IConfigurationService configurationService)
        {
            _publisher = publisher;
            _configurationService = configurationService;
        }

        public MqqtClientStatus GetMqqtClientStatus()
        {
            return this._publisher.GetStatus();
        }

        public Task<MqttSettings> GetMqttBrokerSettings()
        {
            return this._configurationService.GetMqttBrokerSettings();
        }

        /// <summary>
        /// You can use this to check if the application responds.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Ping(string str)
        {
            if (str == "ping")
            {
                return "pong";
            }
            return "what?";
        }

        public void WriteMqttBrokerSettingsAsync(MqttSettings settings)
        {
            this._configurationService.WriteMqttBrokerSettingsAsync(settings);
        }
    }
}
