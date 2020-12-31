using System;
using System.Collections.Generic;
using System.Text;

namespace hass_workstation_service.Communication.InterProcesCommunication.Models
{
    public class MqttSettings
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class MqqtClientStatus
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
    }
}
