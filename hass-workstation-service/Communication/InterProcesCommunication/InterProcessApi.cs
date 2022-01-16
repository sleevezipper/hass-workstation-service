using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.NamedPipe;
using hass_workstation_service.Communication.Util;
using hass_workstation_service.Data;
using hass_workstation_service.Domain.Commands;
using hass_workstation_service.Domain.Sensors;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace hass_workstation_service.Communication.InterProcesCommunication
{
    public class InterProcessApi : IServiceContractInterfaces
    {
        private readonly MqttPublisher _publisher;
        private readonly IConfigurationService _configurationService;

        public InterProcessApi(MqttPublisher publisher, IConfigurationService configurationService)
        {
            _publisher = publisher;
            _configurationService = configurationService;
        }

        public MqqtClientStatus GetMqqtClientStatus() => _publisher.GetStatus();

        public Task<MqttSettings> GetMqttBrokerSettings() => _configurationService.GetMqttBrokerSettings();

        /// <summary>
        /// You can use this to check if the application responds.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Ping(string str) => str == "ping" ? "pong" : "what?";

        public string GetCurrentVersion() => Program.GetVersion();

        /// <summary>
        /// This writes the provided settings to the config file.
        /// </summary>
        /// <param name="settings"></param>
        public void WriteMqttBrokerSettingsAsync(MqttSettings settings) => _configurationService.WriteMqttBrokerSettingsAsync(settings);

        /// <summary>
        /// Enables or disables autostart.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableAutostart(bool enable) => _configurationService.EnableAutoStart(enable);

        public bool IsAutoStartEnabled() => _configurationService.IsAutoStartEnabled();

        public async Task<List<ConfiguredSensorModel>> GetConfiguredSensors()
        {
            var sensors = await _configurationService.GetSensorsAfterLoadingAsync();
            return sensors.Select(s =>
            {
                if (!Enum.TryParse(s.GetType().Name, out AvailableSensors type))
                    Log.Logger.Error("Unknown sensor");

                return new ConfiguredSensorModel(s);
            }).ToList();
        }

        public async Task<ConfiguredSensorModel> GetConfiguredSensor(Guid id)
        {
            var sensors = await _configurationService.GetSensorsAfterLoadingAsync();
            var s = sensors.FirstOrDefault(x => id == x.Id);
            if (s == null)
                return null;
            else
            {
                if (!Enum.TryParse(s.GetType().Name, out AvailableSensors type))
                    Log.Logger.Error("Unknown sensor");

                return new ConfiguredSensorModel(s);
            }
        }

        public List<ConfiguredCommandModel> GetConfiguredCommands()
        {
            return _configurationService.ConfiguredCommands.Select(c =>
            {
                if (!Enum.TryParse(c.GetType().Name, out AvailableCommands type))
                    Log.Logger.Error("Unknown command");

                return new ConfiguredCommandModel(c);
            }).ToList();
        }

        public ConfiguredCommandModel GetConfiguredCommand(Guid id)
        {
            var c = _configurationService.ConfiguredCommands.FirstOrDefault(x => id == x.Id);
            if (c == null)
                return null;
            else
            {
                if (!Enum.TryParse(c.GetType().Name, out AvailableCommands type))
                    Log.Logger.Error("Unknown command");

                return new ConfiguredCommandModel(c);
            }
        }

        public void RemoveSensorById(Guid id) => _configurationService.DeleteConfiguredSensor(id);

        public void RemoveCommandById(Guid id) => _configurationService.DeleteConfiguredCommand(id);

        /// <summary>
        /// Adds a command to the configured commands. This properly initializes the class and writes it to the config file.
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="json"></param>
        public void AddSensor(AvailableSensors sensorType, string json)
        {
            var sensorToCreate = GetSensorToCreate(sensorType, json);

            if (sensorToCreate == null)
                Log.Logger.Error("Unknown sensortype");
            else
                _configurationService.AddConfiguredSensor(sensorToCreate);
        }

        /// <summary>
        /// Adds a command to the configured commands. This properly initializes the class, subscribes to the command topic and writes it to the config file.
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="json"></param>
        public void AddCommand(AvailableCommands commandType, string json)
        {
            var commandToCreate = GetCommandToCreate(commandType, json);

            if (commandToCreate == null)
                Log.Logger.Error("Unknown command type");
            else
                _configurationService.AddConfiguredCommand(commandToCreate);
        }

        public async void UpdateSensorById(Guid id, string json)
        {
            var existingSensor = await GetConfiguredSensor(id);
            var sensorToUpdate = GetSensorToCreate(existingSensor.Type, json);

            if (sensorToUpdate == null)
                Log.Logger.Error("Unknown sensortype");
            else
                _configurationService.UpdateConfiguredSensor(id, sensorToUpdate);
        }

        public void UpdateCommandById(Guid id, string json)
        {
            var existingCommand = GetConfiguredCommand(id);
            var commandToUpdate = GetCommandToCreate(existingCommand.Type, json);

            if (commandToUpdate == null)
                Log.Logger.Error("Unknown commandtype");
            else
                _configurationService.UpdateConfiguredCommand(id, commandToUpdate);
        }

        private AbstractSensor GetSensorToCreate(AvailableSensors sensorType, string json)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new DynamicJsonConverter() }
            };
            dynamic model = JsonSerializer.Deserialize<dynamic>(json, serializerOptions);

            return sensorType switch
            {
                AvailableSensors.UserNotificationStateSensor => new UserNotificationStateSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.DummySensor => new DummySensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.CurrentClockSpeedSensor => new CurrentClockSpeedSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.CPULoadSensor => new CPULoadSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.WMIQuerySensor => new WMIQuerySensor(_publisher, model.Query, (int)model.UpdateInterval, model.Name, scope: model.Scope),
                AvailableSensors.MemoryUsageSensor => new MemoryUsageSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.ActiveWindowSensor => new ActiveWindowSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.WebcamActiveSensor => new WebcamActiveSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.WebcamProcessSensor => new WebcamProcessSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.MicrophoneActiveSensor => new MicrophoneActiveSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.MicrophoneProcessSensor => new MicrophoneProcessSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.NamedWindowSensor => new NamedWindowSensor(_publisher, model.WindowName, model.Name, (int)model.UpdateInterval),
                AvailableSensors.LastActiveSensor => new LastActiveSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.LastBootSensor => new LastBootSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.SessionStateSensor => new SessionStateSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.CurrentVolumeSensor => new CurrentVolumeSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.GPUTemperatureSensor => new GpuTemperatureSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.GPULoadSensor => new GpuLoadSensor(_publisher, (int)model.UpdateInterval, model.Name),
                AvailableSensors.MasterVolumeSensor => new MasterVolumeSensor(_publisher, (int)model.UpdateInterval, model.Name),
                _ => null
            };
        }

        private AbstractCommand GetCommandToCreate(AvailableCommands commandType, string json)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new DynamicJsonConverter() }
            };
            dynamic model = JsonSerializer.Deserialize<dynamic>(json, serializerOptions);

            return commandType switch
            {
                AvailableCommands.ShutdownCommand => new ShutdownCommand(_publisher, model.Name),
                AvailableCommands.RestartCommand => new RestartCommand(_publisher, model.Name),
                AvailableCommands.HibernateCommand => new HibernateCommand(_publisher, model.Name),
                AvailableCommands.LogOffCommand => new LogOffCommand(_publisher, model.Name),
                AvailableCommands.CustomCommand => new CustomCommand(_publisher, model.Command, model.Name),
                AvailableCommands.PlayPauseCommand => new PlayPauseCommand(_publisher, model.Name),
                AvailableCommands.NextCommand => new NextCommand(_publisher, model.Name),
                AvailableCommands.PreviousCommand => new PreviousCommand(_publisher, model.Name),
                AvailableCommands.VolumeUpCommand => new VolumeUpCommand(_publisher, model.Name),
                AvailableCommands.VolumeDownCommand => new VolumeDownCommand(_publisher, model.Name),
                AvailableCommands.MuteCommand => new MuteCommand(_publisher, model.Name),
                AvailableCommands.KeyCommand => new KeyCommand(_publisher, Convert.ToByte(model.Key, 16), model.Name),
                _ => null
            };
        }

        public Task<GeneralSettings> GetGeneralSettings() => _configurationService.ReadGeneralSettings();


        public void WriteGeneralSettings(GeneralSettings settings) => _configurationService.WriteGeneralSettingsAsync(settings);
    }
}
