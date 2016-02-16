using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Sender
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Sender : StatefulService
    {
        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service replica.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            // TODO: If your service needs to handle user requests, return a list of ServiceReplicaListeners here.
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service's partition replica. 
        /// RunAsync executes when the primary replica for this partition has write status.
        /// </summary>
        /// <param name="cancelServicePartitionReplica">Canceled when Service Fabric terminates this partition's replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var queue = await StateManager.GetOrAddAsync<IReliableQueue<string>>("KingQueue");

            while (!cancellationToken.IsCancellationRequested)
            {
                var data = Guid.NewGuid().ToString();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    await queue.EnqueueAsync(tx, data);

                    await tx.CommitAsync();
                }


                ServiceEventSource.Current.Message("Sent: {0}.", data);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}