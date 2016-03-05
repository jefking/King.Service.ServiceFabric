namespace King.Service.ServiceFabric.Tests
{
    using Azure.Data;
    using Microsoft.ServiceFabric.Data;

    public class FakeDequeueService : DequeueService<object>
    {
        public FakeDequeueService(string queueName, IProcessor<object> processor, IReliableStateManager stateManager)
            : base(queueName, processor)
        {
            base.StateManager = stateManager;
        }
    }
}