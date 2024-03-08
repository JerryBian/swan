using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class SwanService2
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISwanDatabase _swanDatabase;
        private readonly ISwanGitFolder _swanGitFolder;
        private readonly ILogger<SwanService2> _logger;

        public SwanService2(
            IMemoryCache memoryCache, 
            ISwanDatabase swanDatabase, 
            ISwanGitFolder swanGitFolder, 
            ILogger<SwanService2> logger)
        {
            _memoryCache = memoryCache;
            _swanDatabase = swanDatabase;
            _swanGitFolder = swanGitFolder;
            _logger = logger;
        }

        public async Task MigrateAsync()
        {

        }
    }
}
