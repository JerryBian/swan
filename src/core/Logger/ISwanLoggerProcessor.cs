using Swan.Core.Model;

namespace Swan.Core.Logger
{
    public interface ISwanLoggerProcessor : IDisposable
    {
        void Ingest(SwanLog log);
    }
}
