namespace King.Service.ServiceFabric.Tests
{
    using NSubstitute;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    public class TaskServiceFake : TaskService
    {
        public TaskServiceFake(IRunnable run)
            : base(Substitute.ForPartsOf<StatelessServiceContext>(), run)
        {
        }

        public async Task RunTest(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);
        }
    }
}