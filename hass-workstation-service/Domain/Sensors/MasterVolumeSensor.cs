using CoreAudio;
using hass_workstation_service.Communication;
using System;
using System.Globalization;

namespace hass_workstation_service.Domain.Sensors
{
    public class MasterVolumeSensor : AbstractSensor
    {
        private MMDeviceEnumerator deviceEnumerator;

        public MasterVolumeSensor(MqttPublisher publisher, int? updateInterval = null, string name = "MasterVolume", Guid id = default(Guid)) : base(publisher, name ?? "CurrentVolume", updateInterval ?? 10, id)
        {
            this.deviceEnumerator = new MMDeviceEnumerator();
        }
        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.ObjectId}/state",
                Icon = "mdi:volume-medium",
                Unit_of_measurement = "%",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        public override string GetState()
        {
            var defaultAudioDevice = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            // check if the volume is muted
            if (defaultAudioDevice.AudioEndpointVolume.Mute) return "0";

            return Math.Round(defaultAudioDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100, 0) // round volume and convert to percent
                .ToString(CultureInfo.InvariantCulture); // convert to string
        }
    }
}