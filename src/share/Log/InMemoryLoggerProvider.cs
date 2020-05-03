using Microsoft.Extensions.Logging;

namespace Laobian.Share.Log
{
    public class InMemoryLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new InMemoryLogger();
        }
    }
}
