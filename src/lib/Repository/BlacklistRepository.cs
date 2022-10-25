using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Microsoft.Extensions.Options;

namespace Laobian.Lib.Repository
{
    public class BlacklistRepository : IBlacklistRepository
    {
        private readonly LaobianOption _option;
        private readonly SemaphoreSlim _semaphoreSlim;

        public BlacklistRepository(IOptions<LaobianOption> option)
        {
            _option = option.Value;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task AddAsync(BlacklistItem item)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var items = await GetItemsAsync();

            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public Task DeleteAsync(BlacklistItem item)
        {
            throw new NotImplementedException();
        }

        public Task<List<BlacklistItem>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        private async Task<List<BlacklistItem>> GetItemsAsync()
        {
            var items = new List<BlacklistItem>();
            var file = Path.Combine(_option.AssetLocation, Constants.FolderAsset, Constants.BlacklistFile);
            if(File.Exists(file))
            {
                var c = await File.ReadAllTextAsync(file);
                items.AddRange(JsonHelper.Deserialize<List<BlacklistItem>>(c));
            }

            return items;
        }
    }
}
