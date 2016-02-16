using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;

namespace Reciever
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Reciever : StatefulService
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
            var queue = await StateManager.GetOrAddAsync<IReliableQueue<string>>("inputQueue");

            while (!cancellationToken.IsCancellationRequested)
            {
                using (ITransaction tx = this.StateManager.CreateTransaction())
                {
                    ConditionalResult<string> dequeuReply = await queue.TryDequeueAsync(tx);

                    if (dequeuReply.HasValue)
                    {
                      //  FileImportValidator.Tell(new ValidateFileCommand(dequeuReply.Value));
                    }

                    ServiceEventSource.Current.Message(dequeuReply.Value);

                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
