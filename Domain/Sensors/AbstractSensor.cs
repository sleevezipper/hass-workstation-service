using System;
using System.Threading.Tasks;
using hass_desktop_service.Communication;
using MQTTnet;

namespace hass_desktop_service.Domain.Sensors
{
    public abstract class AbstractSensor
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public MqttPublisher Publisher { get; protected set; }
        protected AutoDiscoveryConfigModel _autoDiscoveryConfigModel;
        protected AutoDiscoveryConfigModel SetAutoDiscoveryConfigModel(AutoDiscoveryConfigModel config)
        {
            this._autoDiscoveryConfigModel = config;
            return config;
        }

        public abstract AutoDiscoveryConfigModel GetAutoDiscoveryConfig();
        public abstract string GetState();

        public async Task PublishStateAsync()
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(this.GetAutoDiscoveryConfig().State_topic)
            .WithPayload(this.GetState())
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();
            await Publisher.Publish(message);
        }
        public async Task PublishAutoDiscoveryConfigAsync()
        {
            await this.Publisher.PublishAutoDiscoveryConfig(this.GetAutoDiscoveryConfig());
        }

    }
}