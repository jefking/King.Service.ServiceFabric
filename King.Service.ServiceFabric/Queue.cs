namespace King.Service.ServiceFabric
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
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