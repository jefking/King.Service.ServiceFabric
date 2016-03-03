namespace Reciever
{
    using King.Service;
    using King.Service.ServiceFabric;

    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Reciever : RoleService<object>
    {
        public Reciever()
            : base(new RoleTaskManager<object>())
        {
        }
    }
}