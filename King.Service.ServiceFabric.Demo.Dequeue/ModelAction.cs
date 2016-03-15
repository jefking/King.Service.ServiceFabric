namespace King.Service.ServiceFabric.Demo.Dequeue
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Azure.Data;

    public class ModelAction : IProcessor<Model>
    {
        public async Task<bool> Process(Model data)
        {
            Trace.TraceInformation("W00t W00t: {0}{1}", data.Id, data.Name);

            return await Task.FromResult(true);
        }
    }
}