namespace King.Service.ServiceFabric.Demo.Factory
{
    using System.Collections.Generic;

    public class TaskFactory : ITaskFactory<Configuration>
    {
        public IEnumerable<IRunnable> Tasks(Configuration passthrough)
        {
            yield return new Recurring();
        }
    }
}