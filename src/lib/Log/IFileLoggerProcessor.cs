namespace Laobian.Lib.Log
{
    public interface IFileLoggerProcessor : IDisposable
    {
        void Ingest(LaobianLog log);
    }
}
