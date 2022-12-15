namespace Swan.Lib.Log
{
    public interface IFileLoggerProcessor : IDisposable
    {
        void Ingest(SwanLog log);
    }
}
