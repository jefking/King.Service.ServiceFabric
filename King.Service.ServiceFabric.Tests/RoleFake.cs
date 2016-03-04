namespace King.Service.ServiceFabric.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    public class RoleFake<T> : RoleService<T>
    {
        public RoleFake(IRoleTaskManager<T> taskManager, T config = default(T))
            : base(taskManager, config)
        {
        }

        public async Task RunTest(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);
        }
    }
}