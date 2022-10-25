using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Microsoft.Extensions.Options;
using System.Text;

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

        public async Task UpdateAsync(BlacklistItem item)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var items = await GetItemsAsync();
                var existingItem = items.FirstOrDefault(x => x.Ip == item.Ip);
                if (existingItem != null)
                {
                    existingItem.LastUpdateAt = DateTime.Now;
                    existingItem.InvalidTo = item.InvalidTo;
                    existingItem.Reason = item.Reason;
                    if (existingItem.InvalidTo == default)
                    {
                        existingItem.InvalidTo = DateTime.MaxValue;
                    }
                }
                else
                {
                    item.CreateAt = item.LastUpdateAt = DateTime.Now;
                    if (item.InvalidTo == default)
                    {
                        item.InvalidTo = DateTime.MaxValue;
                    }
                    items.Add(item);
                }

                await SaveItemsAsync(items);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task DeleteAsync(string ip)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var items = await GetItemsAsync();
                var existingItem = items.FirstOrDefault(x => x.Ip == ip);
                if(existingItem != null)
                {
                    items.Remove(existingItem);
                    await SaveItemsAsync(items);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<List<BlacklistItem>> GetAllAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var items = await GetItemsAsync();
                return items;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task SaveItemsAsync(List<BlacklistItem> items)
        {
            var file = GetFilePath();
            var content = JsonHelper.Serialize(items.OrderByDescending(x => x.LastUpdateAt), true);
            await File.WriteAllTextAsync(file, content, Encoding.UTF8);
        }

        private string GetFilePath()
        {
            return Path.Combine(_option.AssetLocation, Constants.FolderAsset, Constants.BlacklistFile);
        }

        private async Task<List<BlacklistItem>> GetItemsAsync()
        {
            var items = new List<BlacklistItem>();
            var file = GetFilePath();
            if (File.Exists(file))
            {
                var c = await File.ReadAllTextAsync(file, Encoding.UTF8);
                items.AddRange(JsonHelper.Deserialize<List<BlacklistItem>>(c));
            }

            return items;
        }
    }
}
