namespace King.Service.ServiceFabric.Demo.Dequeue
{
    using Azure.Data;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class ModelAction : IProcessor<Model>
    {
        public async Task<bool> Process(Model data)
        {
            Trace.TraceInformation("Model Data: Id:'{0}', Name: '{1}'", data.Id, data.Name);

            return await Task.FromResult(true);
        }
    }
}