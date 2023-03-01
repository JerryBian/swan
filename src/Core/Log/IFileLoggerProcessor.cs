using Swan.Core.Model.Object;

namespace Swan.Core.Log
{
    public interface IFileLoggerProcessor : IDisposable
    {
        void Ingest(LogObject log);
    }
}
