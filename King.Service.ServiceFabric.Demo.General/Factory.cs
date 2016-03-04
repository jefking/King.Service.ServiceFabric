namespace King.Service.ServiceFabric.Demo.General
{
    using System.Collections.Generic;

    public class Factory : ITaskFactory<Configuration>
    {
        public IEnumerable<IRunnable> Tasks(Configuration passthrough)
        {
            yield return new Recurring();
        }
    }
}