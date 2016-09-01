namespace King.Service.ServiceFabric.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    public class TaskServiceFake : TaskService
    {
        public TaskServiceFake(IRunnable run)
            : base(run)
        {
        }

        public async Task RunTest(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);
        }
    }
}