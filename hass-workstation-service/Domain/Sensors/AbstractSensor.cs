using System;
using System.Threading.Tasks;
using hass_workstation_service.Communication;
using MQTTnet;

namespace hass_workstation_service.Domain.Sensors
{

    public abstract class AbstractSensor
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        public string PreviousPublishedState { get; protected set; }
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
            string state = this.GetState();
            if (this.PreviousPublishedState == state)
            {
                // don't publish the state if it hasn't changed
                return;
            }
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(this.GetAutoDiscoveryConfig().State_topic)
            .WithPayload(state)
            .WithExactlyOnceQoS()
            .WithRetainFlag()
            .Build();
            await Publisher.Publish(message);
            this.PreviousPublishedState = state;
        }
        public async Task PublishAutoDiscoveryConfigAsync()
        {
            await this.Publisher.PublishAutoDiscoveryConfig(this.GetAutoDiscoveryConfig());
        }

    }
}