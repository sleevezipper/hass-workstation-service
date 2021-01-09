using hass_workstation_service.Communication;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace hass_workstation_service.Domain.Sensors
{

    public class MicrophoneActiveSensor : AbstractSensor
    {
        public MicrophoneActiveSensor(MqttPublisher publisher, int? updateInterval = null, string name = "MicrophoneActive", Guid id = default(Guid)) : base(publisher, name ?? "MicrophoneActive", updateInterval ?? 10, id)
        {
        }
        public override string GetState()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return IsMicrophoneInUse() ? "True" : "False";
            }
            else return "unsupported";
        }
        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:microphone",
            });
        }

        [SupportedOSPlatform("windows")]
        private bool IsMicrophoneInUse()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {
                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
