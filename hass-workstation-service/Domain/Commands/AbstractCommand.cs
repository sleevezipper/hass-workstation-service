using System;
using System.Threading.Tasks;
using hass_workstation_service.Communication;
using MQTTnet;

namespace hass_workstation_service.Domain.Commands
{

    public abstract class AbstractCommand : AbstractDiscoverable
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }
        /// <summary>
        /// The update interval in seconds. It checks state only if the interval has passed.
        /// </summary>
        public int UpdateInterval { get; protected set; }
        public DateTime? LastUpdated { get; protected set; }
        public string PreviousPublishedState { get; protected set; }
        public MqttPublisher Publisher { get; protected set; }
        public override string Domain { get => "switch"; }
        public AbstractCommand(MqttPublisher publisher, string name, Guid id = default(Guid))
        {
            if (id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
            }
            else
            {
                this.Id = id;
            }
            this.Name = name;
            this.Publisher = publisher;
            publisher.Subscribe(this);

        }
        protected CommandDiscoveryConfigModel _autoDiscoveryConfigModel;
        protected CommandDiscoveryConfigModel SetAutoDiscoveryConfigModel(CommandDiscoveryConfigModel config)
        {
            this._autoDiscoveryConfigModel = config;
            return config;
        }

        public abstract CommandDiscoveryConfigModel GetAutoDiscoveryConfig();
        public abstract string GetState();

        public async Task PublishStateAsync()
        {
            if (LastUpdated.HasValue && LastUpdated.Value.AddSeconds(this.UpdateInterval) > DateTime.UtcNow)
            {
                // dont't even check the state if the update interval hasn't passed
                return;
            }
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
            this.LastUpdated = DateTime.UtcNow;
        }
        public async void PublishAutoDiscoveryConfigAsync()
        {
            await this.Publisher.AnnounceAutoDiscoveryConfig(this.GetAutoDiscoveryConfig(), this.Domain);
        }
        public async Task UnPublishAutoDiscoveryConfigAsync()
        {
            await this.Publisher.AnnounceAutoDiscoveryConfig(this.GetAutoDiscoveryConfig(), this.Domain, true);
        }
        public abstract void Execute();
    }
}