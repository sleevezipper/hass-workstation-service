using hass_workstation_service.Communication;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Collections.Generic;

namespace hass_workstation_service.Domain.Sensors
{
    public class MicrophoneProcessSensor : AbstractSensor
    {
        private List<string> processes = new List<string>();

        public MicrophoneProcessSensor(MqttPublisher publisher, int? updateInterval = null, string name = "MicrophoneProcess", Guid id = default) : base(publisher, name ?? "MicrophoneProcess", updateInterval ?? 10, id)
        {
        }

        public override string GetState()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return IsMicrophoneInUseRegistry();
            }
            else
            {
                return "unsupported";
            }
        }
        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                NamePrefix = Publisher.NamePrefix,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{DiscoveryConfigModel.GetNameWithPrefix(Publisher.NamePrefix, this.ObjectId)}/state",
                Availability_topic = $"homeassistant/sensor/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        [SupportedOSPlatform("windows")]
        private void CheckLastUsed(RegistryKey key)
        {
            foreach (var subKeyName in key.GetSubKeyNames())
            {
                // NonPackaged has multiple subkeys
                if (subKeyName == "NonPackaged")
                {
                    using (var nonpackagedkey = key.OpenSubKey(subKeyName))
                    {
                        CheckLastUsed(nonpackagedkey);
                    }
                }
                else
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey.GetValueNames().Contains("LastUsedTimeStop"))
                        {
                            var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)subKey.GetValue("LastUsedTimeStop") : -1;
                            if (endTime <= 0)
                            {
                                this.processes.Add(subKeyName);
                            }
                        }
                    }
                }
            }
        }

        [SupportedOSPlatform("windows")]
        private string IsMicrophoneInUseRegistry()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone"))
            {
               CheckLastUsed(key);
            }

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone"))
            {
                CheckLastUsed(key);
            }

            if (this.processes.Count() > 0)
            {
                return String.Join(",", this.processes.ToArray());
            }
            return "off";
        }
    }
}
