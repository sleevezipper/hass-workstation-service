using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class CustomCommand : AbstractCommand
    {
        public string Command { get; protected set; }
        public CustomCommand(MqttPublisher publisher, string command, string name = "Custom", Guid id = default(Guid)) : base(publisher, name ?? "Custom", id)
        {
            this.Command = command;
        }

        public override void Execute()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {this.Command}";
            process.StartInfo = startInfo;
            process.Start();
        }

        public override CommandDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return new CommandDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability",
                Command_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.Name}/set",
                Device = this.Publisher.DeviceConfigModel,
                Expire_after = 60
            };
        }

        public override string GetState()
        {
            return "off";
        }
    }
}
