using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hass_workstation_service.Communication;
using hass_workstation_service.Data;
using hass_workstation_service.Domain.Commands;
using hass_workstation_service.Domain.Sensors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using Serilog;

namespace hass_workstation_service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly MqttPublisher _mqttPublisher;

        public Worker(ILogger<Worker> logger,
            IConfigurationService configuredSensorsService,
            MqttPublisher mqttPublisher)
        {
            _logger = logger;
            this._configurationService = configuredSensorsService;
            this._mqttPublisher = mqttPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _configurationService.ReadCommandSettings(_mqttPublisher);
            _configurationService.ReadSensorSettings(_mqttPublisher);

            while (!_mqttPublisher.IsConnected)
            {
                _logger.LogInformation($"Connecting to MQTT broker...");
                await Task.Delay(2000);
            }
            _logger.LogInformation("Connected. Sending auto discovery messages.");

            _mqttPublisher.AnnounceAvailability("sensor");
            foreach (AbstractSensor sensor in _configurationService.ConfiguredSensors.ToList())
            {
                sensor.PublishAutoDiscoveryConfigAsync();
            }
            foreach (AbstractCommand command in _configurationService.ConfiguredCommands.ToList())
            {
                command.PublishAutoDiscoveryConfigAsync();
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);

                // announce autodiscovery every 30 seconds
                if (_mqttPublisher.LastAvailabilityAnnounce < DateTime.UtcNow.AddSeconds(-10))
                {
                    _mqttPublisher.AnnounceAvailability("sensor");
                }

                foreach (AbstractSensor sensor in _configurationService.ConfiguredSensors.ToList())
                {
                    try
                    {
                        await sensor.PublishStateAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Warning("Sensor failed: " + sensor.Name, ex);
                    }

                }
                foreach (AbstractCommand command in _configurationService.ConfiguredCommands.ToList())
                {
                    try
                    {
                        await command.PublishStateAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Warning("Command state failed: " + command.Name, ex);
                    }

                }
                // announce autodiscovery every 30 seconds
                if (_mqttPublisher.LastConfigAnnounce < DateTime.UtcNow.AddSeconds(-30))
                {
                    foreach (AbstractSensor sensor in _configurationService.ConfiguredSensors.ToList())
                    {
                        sensor.PublishAutoDiscoveryConfigAsync();
                    }
                    foreach (AbstractCommand command in _configurationService.ConfiguredCommands.ToList())
                    {
                        command.PublishAutoDiscoveryConfigAsync();
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _mqttPublisher.AnnounceAvailability("sensor", true);
            await _mqttPublisher.DisconnectAsync();
        }

    }
}
