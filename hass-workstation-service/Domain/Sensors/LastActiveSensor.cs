using hass_workstation_service.Communication;
using System;
using System.Runtime.InteropServices;

namespace hass_workstation_service.Domain.Sensors
{
    public class LastActiveSensor : AbstractSensor
    {
        private DateTime _lastActive = DateTime.MinValue;
        public LastActiveSensor(MqttPublisher publisher, int? updateInterval = 10, string name = "LastActive", Guid id = default) : base(publisher, name ?? "LastActive", updateInterval ?? 10, id){}

        public override SensorDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = this.Name,
                NamePrefix = Publisher.NamePrefix,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/{this.Domain}/{Publisher.DeviceConfigModel.Name}/{DiscoveryConfigModel.GetNameWithPrefix(Publisher.NamePrefix, this.ObjectId)}/state",
                Icon = "mdi:clock-time-three-outline",
                Device_class = "timestamp"
            });
        }

        public override string GetState()
        {
            var lastInput = GetLastInputTime();
            if ((_lastActive - lastInput).Duration().TotalSeconds > 1)
            {
                _lastActive = lastInput;
            }
            return _lastActive.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        }
        

        static DateTime GetLastInputTime()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = Convert.ToInt32(lastInputInfo.dwTime);

                idleTime = envTicks - lastInputTick;
            }


            return idleTime > 0 ? DateTime.Now - TimeSpan.FromMilliseconds(idleTime) : DateTime.Now;
        }


        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }
    }
}
