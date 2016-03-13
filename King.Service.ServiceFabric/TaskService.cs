namespace King.Service.ServiceFabric
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    /// Task Service
    /// </summary>
    /// <typeparam name="T">Task</typeparam>
    public class TaskService : StatelessService
    {
        #region Members
        /// <summary>
        /// Runnable Task
        /// </summary>
        protected readonly IRunnable run;
        #endregion

        #region Constructors
        /// <summary>
        /// Role Service Constructor
        /// </summary>
        /// <param name="run">Task Manager</param>
        public TaskService(IRunnable run)
        {
            if (null == run)
            {
                throw new ArgumentNullException("run");
            }

            this.run = run;
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
            this.run.Start();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
            catch (TaskCanceledException ex)
            {
                Trace.TraceError("Task Canceled Exception, can be normal: {0}", ex);
            }

            this.run.Stop();
        }
        #endregion
    }
}