using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using hass_desktop_service.Communication.Util;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace hass_desktop_service.Communication
{

    public class MqttPublisher
    {
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<MqttPublisher> _logger;
        public DateTime LastConfigAnnounce { get; private set; }
        public DeviceConfigModel DeviceConfigModel { get; private set; }
        public bool IsConnected
        {
            get
            {
                return this._mqttClient.IsConnected;
            }
        }

        public MqttPublisher(
            ILogger<MqttPublisher> logger,
            IMqttClientOptions options,
            DeviceConfigModel deviceConfigModel)
        {

            this._logger = logger;
            this.DeviceConfigModel = deviceConfigModel;


            var factory = new MqttFactory();
            this._mqttClient = factory.CreateMqttClient();

            // connect to the broker
            this._mqttClient.ConnectAsync(options);

            // configure what happens on disconnect
            this._mqttClient.UseDisconnectedHandler(async e =>
            {
                _logger.LogWarning("Disconnected from server");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await this._mqttClient.ConnectAsync(options, CancellationToken.None);
                }
                catch
                {
                    _logger.LogError("Reconnecting failed");
                }
            });
        }

        public async Task Publish(MqttApplicationMessage message)
        {
            if (this._mqttClient.IsConnected)
            {
                await this._mqttClient.PublishAsync(message);
            }
            else
            {
                this._logger.LogInformation($"message dropped because mqtt not connected: {message}");
            }
        }

        public async Task PublishAutoDiscoveryConfig(AutoDiscoveryConfigModel config, bool clearPreviousConfig = false)
        {
            if (this._mqttClient.IsConnected)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new CamelCaseJsonNamingpolicy(),
                    IgnoreNullValues = true,
                    PropertyNameCaseInsensitive = true
                };
                var message = new MqttApplicationMessageBuilder()
                .WithTopic($"homeassistant/sensor/{config.Device.Identifiers}/config")
                .WithPayload(clearPreviousConfig ? "" : JsonSerializer.Serialize(config, options))
                .WithRetainFlag()
                .Build();
                await this.Publish(message);
                LastConfigAnnounce = DateTime.UtcNow;
            }
        }
    }
}
