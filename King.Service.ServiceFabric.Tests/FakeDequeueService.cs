namespace King.Service.ServiceFabric.Tests
{
    using Azure.Data;
    using Microsoft.ServiceFabric.Data;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeDequeueService : DequeueService<object>
    {
        public FakeDequeueService(string queueName, IProcessor<object> processor, IReliableStateManager stateManager)
            : base(queueName, processor)
        {
            base.StateManager = stateManager;
        }

        public async Task RunTest(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);
        }
    }
}