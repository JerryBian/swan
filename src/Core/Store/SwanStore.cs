using GitStoreDotnet;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;

namespace Swan.Core.Store;

public class SwanStore : ISwanStore
{
    private const string ReadGitPath = "obj/read.json";

    private readonly IGitStore _gitStore;
    private readonly SwanOption _swanOption;
    private readonly IMemoryCache _memoryCahce;
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly ILogger<SwanStore> _logger;

    public SwanStore(
        IGitStore gitStore,
        IMemoryCache memoryCache,
        ILogger<SwanStore> logger,
        IOptions<SwanOption> option)
    {
        _logger = logger;
        _gitStore = gitStore;
        _swanOption = option.Value;
        _memoryCahce = memoryCache;
        _semaphoreSlim = new SemaphoreSlim(1, 1);
    }

    public async Task LoadAsync()
    {
        try
        {
            await _semaphoreSlim.WaitAsync();

            if(!_swanOption.SkipGitOperation)
            {
                await _gitStore.PullFromRemoteAsync();
                _logger.LogInformation($"Load from git store to {_swanOption.DataLocation}");
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task SaveAsync(string message)
    {
        try
        {
            await _semaphoreSlim.WaitAsync();

            if(!_swanOption.SkipGitOperation)
            {
                await _gitStore.PushToRemoteAsync(message);
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    #region Swan Read

    public async Task<List<SwanRead>> GetReadItemsAsync(bool admin)
    {
        var obj = await GetSwanObjectAsync(admin);
        return obj.ReadItems;
    }

    public async Task AddReadItemAsync(SwanRead swanRead)
    {
        swanRead.Id = StringHelper.Random();
        swanRead.CreatedAt = swanRead.LastUpdatedAt = DateTime.Now;

        var obj = await GetSwanObjectAsync(true);
        obj.ReadItems.Add(swanRead);

        await _gitStore.InsertOrUpdateAsync(ReadGitPath, JsonHelper.Serialize(obj.ReadItems));
        await SaveAsync($"New Read: {swanRead.BookName}");
        ClearCache();
    }

    public async Task UpdateReadItemAsync(SwanRead swanRead)
    {
        var obj = await GetSwanObjectAsync(true);
        var oldItem = obj.ReadItems.FirstOrDefault(x => x.Id == swanRead.Id);
        if (oldItem == null)
        {
            throw new Exception($"Failed to update read item for id {swanRead.Id}, it does not exist. Raw data: {JsonHelper.Serialize(swanRead)}");
        }

        swanRead.CreatedAt = oldItem.CreatedAt;
        swanRead.LastUpdatedAt = DateTime.Now;
        obj.ReadItems.Remove(oldItem);
        obj.ReadItems.Add(swanRead);
        await _gitStore.InsertOrUpdateAsync(ReadGitPath, JsonHelper.Serialize(obj.ReadItems));
        await SaveAsync($"Update Read: {swanRead.BookName}");
        ClearCache();
    }

    #endregion

    private async Task<SwanObject> GetSwanObjectAsync(bool admin)
    {
        return await _memoryCahce.GetOrCreateAsync(GetCacheKey(admin), async _ =>
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                var obj = new SwanObject();
                var readItemsJson = await _gitStore.GetTextAsync(ReadGitPath);
                if(!string.IsNullOrEmpty(readItemsJson))
                {
                    var readItems = JsonHelper.Deserialize<List<SwanRead>>(readItemsJson);
                    obj.ReadItems.AddRange(readItems);

                    ExtendReadItems(obj);
                }
                
                return obj;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        });
    }

    private void ExtendReadItems(SwanObject obj)
    {
        foreach (var read in obj.ReadItems)
        {
            var metadatas = new List<string>();

            if (!string.IsNullOrEmpty(read.Author))
            {
                if (!string.IsNullOrEmpty(read.AuthorCountry))
                {
                    metadatas.Add($"({read.AuthorCountry}){read.Author}");
                }
                else
                {
                    metadatas.Add(read.Author);
                }
            }

            if (!string.IsNullOrEmpty(read.Translator))
            {
                metadatas.Add($"{read.Translator}(译)");
            }

            read.HtmlMetadata = string.Join(" &middot; ", metadatas);
            read.HtmlComment = MarkdownHelper.ToHtml(read.Comment);
        }
    }

    private void ClearCache()
    {
        _memoryCahce.Remove(GetCacheKey(true));
        _memoryCahce.Remove(GetCacheKey(false));
    }

    private string GetCacheKey(bool admin)
    {
        return $"obj_{admin}";
    }
}
