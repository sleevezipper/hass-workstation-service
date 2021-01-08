using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using hass_workstation_service.Communication.InterProcesCommunication.Models;
using hass_workstation_service.Communication.Util;
using hass_workstation_service.Data;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Serilog;

namespace hass_workstation_service.Communication
{

    public class MqttPublisher
    {
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<MqttPublisher> _logger;
        private readonly IConfigurationService _configurationService;
        private string _mqttClientMessage { get; set; }
        public DateTime LastConfigAnnounce { get; private set; }
        public DeviceConfigModel DeviceConfigModel { get; private set; }
        public bool IsConnected
        {
            get
            {
                if (this._mqttClient == null)
                {
                    return false;
                }
                else
                {
                    return this._mqttClient.IsConnected;
                }
            }
        }

        public MqttPublisher(
            ILogger<MqttPublisher> logger,
            DeviceConfigModel deviceConfigModel,
            IConfigurationService configurationService)
        {

            this._logger = logger;
            this.DeviceConfigModel = deviceConfigModel;
            this._configurationService = configurationService;

            var options = _configurationService.GetMqttClientOptionsAsync().Result;
            _configurationService.MqqtConfigChangedHandler = this.ReplaceMqttClient;

            var factory = new MqttFactory();
            this._mqttClient = factory.CreateMqttClient();

            if (options != null)
            {
                this._mqttClient.ConnectAsync(options);
                this._mqttClientMessage = "Connecting...";
            }
            else
            {
                this._mqttClientMessage = "Not configured";
            }
           
            this._mqttClient.UseConnectedHandler(e => {
                this._mqttClientMessage = "All good";
            });

            // configure what happens on disconnect
            this._mqttClient.UseDisconnectedHandler(async e =>
            {
                this._mqttClientMessage = e.ReasonCode.ToString();
                if (e.ReasonCode != MQTTnet.Client.Disconnecting.MqttClientDisconnectReason.NormalDisconnection)
                {
                    _logger.LogWarning("Disconnected from server");
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    try
                    {
                        await this._mqttClient.ConnectAsync(options, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Reconnecting failed");
                    }
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
                this._logger.LogInformation($"Message dropped because mqtt not connected: {message}");
            }
        }

        public async Task AnnounceAutoDiscoveryConfig(AutoDiscoveryConfigModel config, bool clearConfig = false)
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
                .WithTopic($"homeassistant/sensor/{this.DeviceConfigModel.Name}/{config.Name}/config")
                .WithPayload(clearConfig ? "" : JsonSerializer.Serialize(config, options))
                .WithRetainFlag()
                .Build();
                await this.Publish(message);
                LastConfigAnnounce = DateTime.UtcNow;
            }
        }

        public async void ReplaceMqttClient(IMqttClientOptions options)
        {
            this._logger.LogInformation($"Replacing Mqtt client with new config");
            await _mqttClient.DisconnectAsync();
            try
            {
                await _mqttClient.ConnectAsync(options);
            }
            catch (MqttConnectingFailedException ex)
            {
                this._mqttClientMessage = ex.ResultCode.ToString();
                Log.Logger.Error("Could not connect to broker: " + ex.ResultCode.ToString());
            }
            
        }

        public MqqtClientStatus GetStatus()
        {
            return new MqqtClientStatus() { IsConnected = _mqttClient.IsConnected, Message = _mqttClientMessage };
        }
    }
}
