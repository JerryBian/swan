using Laobian.Share.Logger;

namespace Laobian.Api.Logger
{
    public interface IGitFileLogQueue
    {
        void Add(LaobianLog log);

        bool TryDequeue(out LaobianLog log);
    }
}