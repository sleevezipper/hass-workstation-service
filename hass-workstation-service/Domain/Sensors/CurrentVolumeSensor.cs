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
        private MMDeviceEnumerator deviceEnumerator;
        private MMDeviceCollection devices;
        public CurrentVolumeSensor(MqttPublisher publisher, int? updateInterval = null, string name = "CurrentVolume", Guid id = default(Guid)) : base(publisher, name ?? "CurrentVolume", updateInterval ?? 10, id) {
            this.deviceEnumerator = new MMDeviceEnumerator();
            this.devices = deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);
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
                Icon = "mdi:volume-medium",
                Unit_of_measurement = "%",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        public override string GetState()
        {
            List<float> peaks = new List<float>();

            foreach (MMDevice device in devices)
            {
                peaks.Add(device.AudioMeterInformation.PeakValues[0]);
            }

            return Math.Round(peaks.Max() * 100, 0).ToString(CultureInfo.InvariantCulture);
        }


    }
}
