namespace King.Service.ServiceFabric.Demo.General
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class General : RoleService<Configuration>
    {
        public General()
            : base(new RoleTaskManager<Configuration>(new Factory()))
        {
        }
    }
}
