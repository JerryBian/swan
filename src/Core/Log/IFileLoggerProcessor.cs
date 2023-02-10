namespace Swan.Core.Log
{
    public interface IFileLoggerProcessor : IDisposable
    {
        void Ingest(SwanLog log);
    }
}
