﻿using hass_workstation_service.Communication;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Commands
{
    public class CustomCommand : AbstractCommand
    {
        public string Command { get; protected set; }
        public Process Process { get; private set; }
        public CustomCommand(MqttPublisher publisher, string command, string name = "Custom", Guid id = default(Guid)) : base(publisher, name ?? "Custom", id)
        {
            this.Command = command;
        }

        public override async void Press()
        {
            this.Process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {this.Command}";
            this.Process.StartInfo = startInfo;
            
            // turn off the sensor to guarantee disable the switch
            // useful if command changes power state of device
            
            try
            {
                this.Process.Start();
            }
            catch (Exception e)
            {
                Log.Logger.Error($"Sensor {this.Name} failed", e);
            }
        }



        public override CommandDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return new CommandDiscoveryConfigModel()
            {
                Name = this.Name,
                NamePrefix = Publisher.NamePrefix,
                Unique_id = this.Id.ToString(),
                Availability_topic = $"homeassistant/sensor/{Publisher.DeviceConfigModel.Name}/availability",
                Command_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{Publisher.NamePrefix}{this.ObjectId}/set",
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{DiscoveryConfigModel.GetNameWithPrefix(Publisher.NamePrefix, this.ObjectId)}/state",
                Device = this.Publisher.DeviceConfigModel,
            };
        }
    }
}
