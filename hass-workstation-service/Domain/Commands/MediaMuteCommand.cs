using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class MediaMuteCommand : KeyCommand
    {
        public MediaMuteCommand(MqttPublisher publisher, string name = "Mute", Guid id = default(Guid)) : base(publisher, KeyCommand.VK_VOLUME_MUTE, name ?? "Mute", id) { }
    }
}
