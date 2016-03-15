namespace King.Service.ServiceFabric.Demo.Task
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class Service : TaskService
    {
        public Service() : base(new Recurring()) { }
    }
}