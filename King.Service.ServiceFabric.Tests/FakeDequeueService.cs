namespace King.Service.ServiceFabric.Tests
{
    using King.Azure.Data;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeDequeueService : DequeueService<object>
    {
        public FakeDequeueService(StatefulServiceContext context, string queueName, IProcessor<object> processor)
            : base(context, queueName, processor)
        {
        }

        public async Task RunTest(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);
        }
    }
}