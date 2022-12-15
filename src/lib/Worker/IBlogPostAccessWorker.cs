using Swan.Lib.Model;

namespace Swan.Lib.Worker
{
    public interface IBlogPostAccessWorker
    {
        void Add(PostAccessItem item);

        Task ProcessAsync();

        Task StopAsync();
    }
}
