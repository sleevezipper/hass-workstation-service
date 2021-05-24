using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class NextCommand : KeyCommand
    {
        public NextCommand(MqttPublisher publisher, string name = "Next", Guid id = default(Guid)) : base(publisher, KeyCommand.VK_MEDIA_NEXT_TRACK, name ?? "Next", id) { }
    }
}
