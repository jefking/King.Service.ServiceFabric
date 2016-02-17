namespace Reciever
{
    using King.Service.ServiceFabric;

    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Reciever : DequeueService<string>
    {
        public Reciever()
            : base("KingQueue", new DataProcessor())
        {
        }
    }
}