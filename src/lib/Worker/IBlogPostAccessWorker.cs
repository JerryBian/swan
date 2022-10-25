using Laobian.Lib.Model;

namespace Laobian.Lib.Worker
{
    public interface IBlogPostAccessWorker
    {
        void Add(PostAccessItem item);

        Task ProcessAsync();

        Task StopAsync();
    }
}
