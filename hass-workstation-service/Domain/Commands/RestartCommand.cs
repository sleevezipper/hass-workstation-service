using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class RestartCommand : CustomCommand
    {
        public RestartCommand(MqttPublisher publisher, string name = "Shutdown", Guid id = default(Guid)) : base(publisher, "shutdown /r", name ?? "Restart", id)
        {
        }
    }
}
