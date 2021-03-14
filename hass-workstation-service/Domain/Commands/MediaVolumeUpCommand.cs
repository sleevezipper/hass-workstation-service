using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class MediaVolumeUpCommand : KeyCommand
    {
        public MediaVolumeUpCommand(MqttPublisher publisher, string name = "VolumeUp", Guid id = default(Guid)) : base(publisher, KeyCommand.VK_VOLUME_UP, name ?? "VolumeUp", id) { }
    }
}
