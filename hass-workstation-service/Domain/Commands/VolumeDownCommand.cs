using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class VolumeDownCommand : KeyCommand
    {
        public VolumeDownCommand(MqttPublisher publisher, string name = "VolumeDown", Guid id = default(Guid)) : base(publisher, KeyCommand.VK_VOLUME_DOWN, name ?? "VolumeDown", id) { }
    }
}
