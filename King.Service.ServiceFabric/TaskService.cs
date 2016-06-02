﻿namespace King.Service.ServiceFabric
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Runtime;
    using System.Fabric;
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
        /// <param name="serviceContext">Service Context</param>
        /// <param name="run">Task Manager</param>
        public TaskService(StatelessServiceContext serviceContext, IRunnable run)
            :base(serviceContext)
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
            if (this.run.Start())
            {

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

                if (this.run.Stop())
                {
                    Trace.TraceError("Task stopped successfully.");
                }
                else
                {
                    Trace.TraceError("Task failed to stop: did not complete stop process successfully.");
                }
            }
            else
            {
                Trace.TraceError("Task failed to start: did not complete start process successfully.");
            }
        }
        #endregion
    }
}