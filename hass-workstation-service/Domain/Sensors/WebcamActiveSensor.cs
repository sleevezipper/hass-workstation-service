using hass_workstation_service.Communication;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace hass_workstation_service.Domain.Sensors
{
    public class WebcamActiveSensor : AbstractSensor
    {
        public WebcamActiveSensor(MqttPublisher publisher, int? updateInterval = null, string name = "WebcamActive", Guid id = default) : base(publisher, name ?? "WebcamActive", updateInterval ?? 10, id)
        {
        }
        public override string GetState()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return IsWebCamInUseRegistry() ? "True" : "False";
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
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:webcam",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability",
                Expire_after = 60
            });
        }

        [SupportedOSPlatform("windows")]
        private bool IsWebCamInUseRegistry()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    // NonPackaged has multiple subkeys
                    if (subKeyName == "NonPackaged")
                    {
                        using (var nonpackagedkey = key.OpenSubKey(subKeyName))
                        {
                            foreach (var nonpackagedSubKeyName in nonpackagedkey.GetSubKeyNames())
                            {
                                using (var subKey = nonpackagedkey.OpenSubKey(nonpackagedSubKeyName))
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
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    // NonPackaged has multiple subkeys
                    if (subKeyName == "NonPackaged")
                    {
                        using (var nonpackagedkey = key.OpenSubKey(subKeyName))
                        {
                            foreach (var nonpackagedSubKeyName in nonpackagedkey.GetSubKeyNames())
                            {
                                using (var subKey = nonpackagedkey.OpenSubKey(nonpackagedSubKeyName))
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
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
