namespace King.Service.ServiceFabric
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    /// Dequeue Service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DequeueService<T> : StatefulService
    {
        #region Members
        /// <summary>
        /// Processor
        /// </summary>
        protected readonly IProcessor<T> processor;

        /// <summary>
        /// Queue Name
        /// </summary>
        protected readonly string queueName;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="queueName">Queue Name</param>
        /// <param name="processor">Processor</param>
        public DequeueService(string queueName, IProcessor<T> processor)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("queueName");
            }
            if (null == processor)
            {
                throw new ArgumentNullException("processor");
            }

            this.processor = processor;
            this.queueName = queueName;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Run Async
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task</returns>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var queue = await this.StateManager.GetOrAddAsync<IReliableQueue<T>>(this.queueName);

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var message = await queue.TryDequeueAsync(tx);

                    if (message.HasValue)
                    {
                        var success = await this.processor.Process(message.Value);

                        if (success)
                        {
                            await tx.CommitAsync();
                        }
                    }
                }
            }
            else
            {
                Trace.TraceInformation("Dequeue not run, cancelation called.");
            }
        }
        #endregion
    }
}