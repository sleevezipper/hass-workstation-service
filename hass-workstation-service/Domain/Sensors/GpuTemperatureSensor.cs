using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hass_workstation_service.Communication;
using LibreHardwareMonitor.Hardware;

namespace hass_workstation_service.Domain.Sensors
{
    public class GpuTemperatureSensor : AbstractSensor
    {
        private Computer _computer;
        private IHardware _gpu;
        public GpuTemperatureSensor(MqttPublisher publisher, int? updateInterval = null, string name = "GPUTemperature", Guid id = default(Guid)) : base(publisher, name ?? "GPUTemperature", updateInterval ?? 10, id)
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            _computer.Open();
            this._gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuAmd || h.HardwareType == HardwareType.GpuNvidia);
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{this.Name}/state",
                Device_class = "temperature",
                Unit_of_measurement = "°C",
                Availability_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/availability"
            });
        }

        public override string GetState()
        {
            if (_gpu == null)
            {
                return "NotSupported";
            }
            _gpu.Update();
            var sensor = _gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
            if (sensor == null)
            {
                return "NotSupported";
            }

            return sensor.Value.HasValue ? sensor.Value.Value.ToString("#.##", CultureInfo.InvariantCulture) : "Unknown";
        }
    }
}
