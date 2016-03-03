namespace King.Service.ServiceFabric
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    public class DequeueService<T> : StatefulService
    {
        #region Members
        protected readonly IProcessor<T> processor;
        protected readonly string name;
        #endregion

        protected DequeueService(string name, IProcessor<T> processor)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("");
            }
            if (null == processor)
            {
                throw new ArgumentNullException("");
            }

            this.processor = processor;
            this.name = name;
        }

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
            var queue = await StateManager.GetOrAddAsync<IReliableQueue<T>>(name);

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var tx = this.StateManager.CreateTransaction())
                {
                    var dequeuReply = await queue.TryDequeueAsync(tx);

                    if (dequeuReply.HasValue)
                    {
                        var success = await this.processor.Process(dequeuReply.Value);
                        if (success)
                        {
                            await tx.CommitAsync();
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}