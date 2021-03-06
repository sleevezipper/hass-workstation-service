using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class MediaPreviousCommand : KeyCommand
    {
        public MediaPreviousCommand(MqttPublisher publisher, string name = "Previous", Guid id = default(Guid)) : base(publisher, KeyCommand.VK_MEDIA_PREV_TRACK, name ?? "Previous", id) { }
    }
}
