namespace King.Service.ServiceFabric.Demo.Task
{
    using System.Diagnostics;
    using King.Service;

    public class Recurring : RecurringTask
    {
        public override void Run()
        {
            Trace.TraceInformation("working");
        }
    }
}