using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class LogOffCommand : CustomCommand
    {
        public LogOffCommand(MqttPublisher publisher, string name = "Shutdown", Guid id = default(Guid)) : base(publisher, "shutdown /l", name ?? "LogOff", id)
        {
        }
    }
}
