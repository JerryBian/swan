using Microsoft.Extensions.Options;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Option;
using Swan.Lib.Model;
using System.Runtime.CompilerServices;

namespace Swan.Lib.Repository
{
    public class BlogRepository : BaseRepository, IBlogRepository
    {
        private const string FileExt = ".json";

        private readonly SwanOption _option;
        private readonly ILogger<BlogRepository> _logger;
        private readonly SemaphoreSlim _semaphoreSlim;

        public BlogRepository(IOptions<SwanOption> option, ILogger<BlogRepository> logger)
        {
            _logger = logger;
            _option = option.Value;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> AddPostAccessAsync(string id, int count, CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                string baseDir = GetPostDir();
                string path = Path.Combine(baseDir, $"{id}{FileExt}");
                if (!File.Exists(path))
                {
                    _logger.LogWarning($"Add access count failed, path {path} not found.");
                    return false;
                }

                string content = await File.ReadAllTextAsync(path);
                BlogPost post = JsonHelper.Deserialize<BlogPost>(content);
                post.AccessCount += count;
                await WriteAsync(path, JsonHelper.Serialize(post, true));
                return true;
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async IAsyncEnumerable<BlogPost> ReadAllPostsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            try
            {
                string baseDir = GetPostDir();
                foreach (string item in await ReadAllAsync(baseDir, $"*{FileExt}", cancellationToken))
                {
                    BlogPost post = JsonHelper.Deserialize<BlogPost>(await File.ReadAllTextAsync(item, cancellationToken));
                    yield return post;
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task AddPostAsync(BlogPost item, CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            try
            {
                await AddPostCoreAsync(item, cancellationToken);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _ = _semaphoreSlim.Release();
                }
            }
        }

        private async Task AddPostCoreAsync(BlogPost item, CancellationToken cancellationToken)
        {
            string baseDir = GetPostDir();
            if (item.CreateTime == default)
            {
                item.CreateTime = DateTime.Now;
            }

            if (item.PublishTime == default)
            {
                item.PublishTime = item.CreateTime.AddHours(1);
            }

            if (string.IsNullOrEmpty(item.Title))
            {
                item.Title = "Untitled";
            }

            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = StringHelper.Random();
            }

            if (string.IsNullOrEmpty(item.Link))
            {
                item.Link = StringHelper.Random();
            }

            item.LastUpdateTime = DateTime.Now;

            string path = Path.Combine(baseDir, $"{item.Id}{FileExt}");
            if (File.Exists(path))
            {
                BlogPost p = JsonHelper.Deserialize<BlogPost>(File.ReadAllText(path));
                item.AccessCount = Math.Max(item.AccessCount, p.AccessCount);
            }

            await WriteAsync(path, JsonHelper.Serialize(item, true), cancellationToken);
        }

        public async Task UpdatePostAsync(BlogPost item, CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();

            try
            {
                string baseDir = GetPostDir();
                if (string.IsNullOrEmpty(item.Id))
                {
                    throw new Exception("Missing post id.");
                }

                await AddPostCoreAsync(item, cancellationToken);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _ = _semaphoreSlim.Release();
                }
            }
        }

        private string GetPostDir()
        {
            string path = Path.Combine(_option.AssetLocation, "asset", "blog", "post");
            _ = Directory.CreateDirectory(path);
            return path;
        }

        private string GetTempPostDir()
        {
            string path = Path.Combine(_option.AssetLocation, "asset", "temp", "post");
            _ = Directory.CreateDirectory(path);
            return path;
        }
    }
}
