namespace King.Service.ServiceFabric
{
    using King.Azure.Data;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Dequeue Service
    /// </summary>
    /// <typeparam name="T">Dequeue Type</typeparam>
    public class DequeueService<T>
    {
        #region Members
        /// <summary>
        /// Processor
        /// </summary>
        protected readonly IProcessor<T> processor;

        /// <summary>
        /// State
        /// </summary>
        protected readonly IReliableStateManager state;

        /// <summary>
        /// Queue Name
        /// </summary>
        protected readonly string queueName;

        /// <summary>
        /// Timing
        /// </summary>
        protected readonly int seconds = 15;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="state">State</param>
        /// <param name="queueName">Queue Name</param>
        /// <param name="processor">Processor</param>
        /// <param name="seconds">Check every</param>
        public DequeueService(IReliableStateManager state, string queueName, IProcessor<T> processor, int seconds = 15)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("queueName");
            }
            if (null == processor)
            {
                throw new ArgumentNullException("processor");
            }

            this.state = state;
            this.processor = processor;
            this.queueName = queueName;
            this.seconds = 0 > seconds ? 15 : seconds;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Run Async
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task</returns>
        protected async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var queue = await this.state.GetOrAddAsync<IReliableQueue<T>>(this.queueName);

                        using (var tx = this.state.CreateTransaction())
                        {
                            var message = await queue.TryDequeueAsync(tx);

                            if (message.HasValue)
                            {
                                var success = await this.processor.Process(message.Value);

                                if (success)
                                {
                                    await tx.CommitAsync();
                                }
                                else
                                {
                                    Trace.TraceWarning("Message was not processed successfully.");
                                }
                            }
                            else
                            {
                                Trace.TraceInformation("Message does not contain a value.");
                            }
                        }

                        await Task.Delay(TimeSpan.FromSeconds(this.seconds), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Processing exeption: {0}", ex);
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError("Task Canceled Exception (might be normal): '{0}'.", ex);
            }
        }
        #endregion
    }
}