namespace King.Service.ServiceFabric
{
    using System;
    using System.Diagnostics;
    using System.Fabric;
    using System.Threading;

    /// <summary>
    /// Run Time
    /// </summary>
    /// <typeparam name="T">Service Type</typeparam>
    public class RunTime<T>
    {
        #region Members
        /// <summary>
        /// service type name
        /// </summary>
        protected readonly string serviceTypeName;
        #endregion


        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">service type name</param>
        public RunTime(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.serviceTypeName = name;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Run
        /// </summary>
        public void Run()
        {
            using (var fabricRuntime = FabricRuntime.Create())
            {
                fabricRuntime.RegisterServiceType(serviceTypeName, typeof(T));

                Trace.TraceInformation("Service host process {0} registered service type {1}", Process.GetCurrentProcess().Id, typeof(T).Name);

                Thread.Sleep(Timeout.Infinite);
            }
        }
        #endregion
    }
}