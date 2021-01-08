using hass_workstation_service.Communication;
using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace hass_workstation_service.Domain.Sensors
{
    public enum DetectionMode
    {
        Registry,
        OpenCV
    }
    public class WebcamActiveSensor : AbstractSensor
    {
        public DetectionMode DetectionMode { get; private set; }
        public WebcamActiveSensor(MqttPublisher publisher, int? updateInterval = null, string name = "WebcamActive", DetectionMode detectionMode = DetectionMode.Registry, Guid id = default(Guid)) : base(publisher, name, updateInterval ?? 10, id)
        {
            this.DetectionMode = detectionMode;
        }
        public override string GetState()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                switch (this.DetectionMode)
                {
                    case DetectionMode.Registry:
                        return IsWebCamInUseRegistry() ? "True" : "False";
                    case DetectionMode.OpenCV:
                        return IsWebCamInUseOpenCV() ? "True" : "False";
                    default:
                        return "Error";
                }
            }
            else
            {
                return "unsopported";
            }
        }
        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:webcam",
            });
        }

        private bool IsWebCamInUseOpenCV()
        {
            try
            {
                VideoCapture capture = new VideoCapture(0);
                OutputArray image = OutputArray.Create(new Mat());

                // capture.Read() return false if it doesn't succeed in capturing
                if (capture.Read(image))
                {
                    capture.Release();
                    capture.Dispose();
                    return false;
                }
                else
                {
                    capture.Release();
                    capture.Dispose();
                    return true;
                }

            }
            catch (Exception)
            {

                return false;
            }
        }

        [SupportedOSPlatform("windows")]
        private bool IsWebCamInUseRegistry()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged"))
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
