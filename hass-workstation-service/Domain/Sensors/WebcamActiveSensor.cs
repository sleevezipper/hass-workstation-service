using hass_workstation_service.Communication;
using OpenCvSharp;
using System;

namespace hass_workstation_service.Domain.Sensors
{
    public class WebcamActiveSensor : AbstractSensor
    {
        public WebcamActiveSensor(MqttPublisher publisher, int? updateInterval = null, string name = "WebcamActive", Guid id = default) : base(publisher, name, updateInterval ?? 10, id)
        {
        }
        public override string GetState()
        {
            return IsWebCamInUse() ? "True" : "False";
        }
        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{this.Name}/state",
                Icon = "mdi:webcam",
            });
        }

        private bool IsWebCamInUse()
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
    }
}
