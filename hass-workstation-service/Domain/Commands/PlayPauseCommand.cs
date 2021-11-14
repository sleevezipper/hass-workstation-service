using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class PlayPauseCommand : KeyCommand
    {
        public PlayPauseCommand(MqttPublisher publisher, string name = "PlayPause", Guid id = default(Guid)) : base(publisher, KeyCommand.VK_MEDIA_PLAY_PAUSE, name ?? "PlayPause", id) { }
    }
}
