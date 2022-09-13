using Laobian.Lib.Extension;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

namespace Laobian.Lib.Repository
{
    public class BlogRepository : BaseRepository, IBlogRepository
    {
        private const string FileExt = ".json";

        private readonly LaobianOption _option;
        private readonly ILogger<BlogRepository> _logger;
        private readonly SemaphoreSlim _semaphoreSlim;

        public BlogRepository(IOptions<LaobianOption> option, ILogger<BlogRepository> logger)
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
                if(!File.Exists(path))
                {
                    _logger.LogWarning($"Add access count failed, path {path} not found.");
                    return false;
                }

                var content = await File.ReadAllTextAsync(path);
                var post = JsonHelper.Deserialize<BlogPost>(content);
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
                    var post = JsonHelper.Deserialize<BlogPost>(await File.ReadAllTextAsync(item, cancellationToken));
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

            if(string.IsNullOrEmpty(item.Link))
            {
                item.Link = StringHelper.Random();
            }

            item.LastUpdateTime = DateTime.Now;

            string path = Path.Combine(baseDir, $"{item.Id}{FileExt}");
            if(File.Exists(path))
            {
                var p = JsonHelper.Deserialize<BlogPost>(File.ReadAllText(path));
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
    }
}
