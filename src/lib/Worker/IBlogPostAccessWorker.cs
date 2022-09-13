namespace Laobian.Lib.Worker
{
    public interface IBlogPostAccessWorker
    {
        void Add(string id);

        Task ProcessAsync();

        Task StopAsync();
    }
}
