using Swan.Core.Model;

namespace Swan.Core.Logger
{
    public interface IGitFileLoggerProcessor : IDisposable
    {
        void Ingest(SwanLog log);
    }
}
