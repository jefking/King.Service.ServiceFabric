namespace Reciever
{
    using System.Threading.Tasks;
    using King.Azure.Data;

    public class DataProcessor : IProcessor<string>
    {
        public Task<bool> Process(string data)
        {
            ServiceEventSource.Current.Message("Recieved: {0}.", data);

            return new TaskFactory().StartNew(() => { return true; });
        }
    }
}