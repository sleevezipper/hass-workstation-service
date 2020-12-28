using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hass_desktop_service.Communication;
using hass_desktop_service.Data;
using hass_desktop_service.Domain.Sensors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;

namespace hass_desktop_service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ConfiguredSensorsService _configuredSensorsService;
        private readonly MqttPublisher _mqttPublisher;

        public Worker(ILogger<Worker> logger,
            ConfiguredSensorsService configuredSensorsService,
            MqttPublisher mqttPublisher)
        {
            _logger = logger;
            this._configuredSensorsService = configuredSensorsService;
            this._mqttPublisher = mqttPublisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!_mqttPublisher.IsConnected)
            {
                _logger.LogInformation("Connecting to MQTT broker...");
                await Task.Delay(2000);
            }
            _logger.LogInformation("Connected. Sending auto discovery messages.");
            foreach (AbstractSensor sensor in _configuredSensorsService.ConfiguredSensors)
            {
                await sensor.PublishAutoDiscoveryConfigAsync();
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);

                foreach (AbstractSensor sensor in _configuredSensorsService.ConfiguredSensors)
                {
                    await sensor.PublishStateAsync();
                }
                // announce autodiscovery every 30 seconds
                if (_mqttPublisher.LastConfigAnnounce < DateTime.UtcNow.AddSeconds(-30))
                {
                    // TODO: make every sensor publish its auto discovery config
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
