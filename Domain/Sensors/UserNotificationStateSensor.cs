using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using hass_workstation_service.Communication;

namespace hass_workstation_service.Domain.Sensors
{
    public class UserNotificationStateSensor : AbstractSensor
    {
        public UserNotificationStateSensor(MqttPublisher publisher, string name = "NotificationState")
        {
            this.Id = new Guid();
            this.Name = name;
            this.Publisher = publisher;
        }

        public UserNotificationStateSensor(MqttPublisher publisher, string name, Guid id)
        {
            this.Id = id;
            this.Name = name;
            this.Publisher = publisher;
        }
        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{this.Name}/state",
                Icon = "mdi:laptop",
            });
        }

        public override string GetState()
        {
            return GetStateEnum().ToString();
        }

        [DllImport("shell32.dll")]
        static extern int SHQueryUserNotificationState(out UserNotificationState state);

        public UserNotificationState GetStateEnum()
        {
            SHQueryUserNotificationState(out UserNotificationState state);

            return state;
        }
    }

    public enum UserNotificationState
    {
        /// <summary>
        /// A screen saver is displayed, the machine is locked,
        /// or a nonactive Fast User Switching session is in progress.
        /// </summary>
        NotPresent = 1,

        /// <summary>
        /// A full-screen application is running or Presentation Settings are applied.
        /// Presentation Settings allow a user to put their machine into a state fit
        /// for an uninterrupted presentation, such as a set of PowerPoint slides, with a single click.
        /// </summary>
        Busy = 2,

        /// <summary>
        /// A full-screen (exclusive mode) Direct3D application is running.
        /// </summary>
        RunningDirect3dFullScreen = 3,

        /// <summary>
        /// The user has activated Windows presentation settings to block notifications and pop-up messages.
        /// </summary>
        PresentationMode = 4,

        /// <summary>
        /// None of the other states are found, notifications can be freely sent.
        /// </summary>
        AcceptsNotifications = 5,

        /// <summary>
        /// Introduced in Windows 7. The current user is in "quiet time", which is the first hour after
        /// a new user logs into his or her account for the first time. During this time, most notifications
        /// should not be sent or shown. This lets a user become accustomed to a new computer system
        /// without those distractions.
        /// Quiet time also occurs for each user after an operating system upgrade or clean installation.
        /// </summary>
        QuietTime = 6,

        /// <summary>
        /// Introduced in Windows 7. The current user is in "quiet time", which is the first hour after
        /// a new user logs into his or her account for the first time. During this time, most notifications
        /// should not be sent or shown. This lets a user become accustomed to a new computer system
        /// without those distractions.
        /// Quiet time also occurs for each user after an operating system upgrade or clean installation.
        /// </summary>
        RunningWindowsStoreApp = 7
    };
}