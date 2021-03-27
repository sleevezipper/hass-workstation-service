using System;

namespace hass_workstation_service.Communication.InterProcesCommunication.Models
{
    public class MqttSettings
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? Port { get; set; }
        public bool UseTLS { get; set; }
    }

    public class MqqtClientStatus
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
    }

    public class ConfiguredSensorModel
    {
        public Guid Id { get; set; }
        public AvailableSensors Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int UpdateInterval { get; set; }
        public string UnitOfMeasurement { get; set; }
    }

    public class ConfiguredCommandModel
    {
        public Guid Id { get; set; }
        public AvailableCommands Type { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
    }

    public enum AvailableSensors
    {
        UserNotificationStateSensor,
        DummySensor,
        CurrentClockSpeedSensor,
        CPULoadSensor,
        WMIQuerySensor,
        MemoryUsageSensor,
        WebcamActiveSensor,
        MicrophoneActiveSensor,
        ActiveWindowSensor,
        NamedWindowSensor,
        LastActiveSensor,
        LastBootSensor,
        SessionStateSensor,
        CurrentVolumeSensor,
        GPUTemperatureSensor,
        GPULoadSensor
    }

    public enum AvailableCommands
    {
        CustomCommand,
        ShutdownCommand,
        LogOffCommand,
        RestartCommand,
        KeyCommand,
        PlayPauseCommand,
        NextCommand,
        PreviousCommand,
        VolumeUpCommand,
        VolumeDownCommand,
        MuteCommand
    }
}