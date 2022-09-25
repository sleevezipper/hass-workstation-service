using System;
using System.Threading.Tasks;
using hass_workstation_service.Communication;
using MQTTnet;

namespace hass_workstation_service.Domain.Commands
{
    public abstract class AbstractCommand : AbstractDiscoverable
    {
        /// <summary>
        /// The update interval in seconds. It checks state only if the interval has passed.
        /// </summary>
        public static int UpdateInterval { get => 1; }
        public DateTime? LastUpdated { get; protected set; }
        public string PreviousPublishedState { get; protected set; }
        public MqttPublisher Publisher { get; protected set; }
        public override string Domain { get => "button"; }

        public AbstractCommand(MqttPublisher publisher, string name, Guid id = default)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Name = name;
            Publisher = publisher;
            publisher.Subscribe(this);
        }

        public async Task PublishStateAsync()
        {
            // dont't even check the state if the update interval hasn't passed
            if (LastUpdated.HasValue && LastUpdated.Value.AddSeconds(UpdateInterval) > DateTime.UtcNow)
                return;

            string state = "";
            // don't publish the state if it hasn't changed
            if (PreviousPublishedState == state)
                return;

            var message = new MqttApplicationMessageBuilder()
                                                .WithTopic(GetAutoDiscoveryConfig().State_topic)
                                                .WithPayload(state)
                                                //.WithExactlyOnceQoS()
                                                //.WithRetainFlag()
                                                .Build();
            await Publisher.Publish(message);
            PreviousPublishedState = state;
            LastUpdated = DateTime.UtcNow;
        }

        public async void PublishAutoDiscoveryConfigAsync() => await Publisher.AnnounceAutoDiscoveryConfig(this);

        public async Task UnPublishAutoDiscoveryConfigAsync() => await Publisher.AnnounceAutoDiscoveryConfig(this, true);

        protected CommandDiscoveryConfigModel _autoDiscoveryConfigModel;
        protected CommandDiscoveryConfigModel SetAutoDiscoveryConfigModel(CommandDiscoveryConfigModel config)
        {
            _autoDiscoveryConfigModel = config;
            return config;
        }

        public abstract void Press();
    }
}