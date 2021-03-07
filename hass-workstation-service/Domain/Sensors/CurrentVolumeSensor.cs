using CoreAudio;
using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace hass_workstation_service.Domain.Sensors
{
    public class CurrentVolumeSensor : AbstractSensor
    {
        private MMDeviceEnumerator DevEnum;
        public CurrentVolumeSensor(MqttPublisher publisher, int? updateInterval = null, string name = "CurrentVolume", Guid id = default(Guid)) : base(publisher, name ?? "CurrentVolume", updateInterval ?? 10, id) {
            this.DevEnum = new MMDeviceEnumerator();
        }
        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Icon = "mdi:volume-medium",
                Unit_of_measurement = "%",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        public override string GetState()
        {
            var collection = DevEnum.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);

            List<float> peaks = new List<float>();


            foreach (MMDevice device in collection)
            {
                peaks.Add(device.AudioMeterInformation.PeakValues[0]);
            }

            return Math.Round(peaks.Max() * 100, 0).ToString(CultureInfo.InvariantCulture);
        }


    }
}
