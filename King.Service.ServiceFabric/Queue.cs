namespace King.Service.ServiceFabric
{
    using Microsoft.ServiceFabric.Data;

    public class Queue
    {
        protected readonly IReliableStateManager manager = null;

        public Queue(IReliableStateManager manager)
        {
            this.manager = manager;
        }
        
    }
}