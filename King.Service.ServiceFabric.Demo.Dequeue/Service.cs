namespace King.Service.ServiceFabric.Demo.Dequeue
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Service : DequeueService<Model>
    {
        public Service() : base("cool", new ModelAction()) { }
    }
}