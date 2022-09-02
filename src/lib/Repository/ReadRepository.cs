using Laobian.Lib.Extension;
using Laobian.Lib.Helper;
using Laobian.Lib.Model;
using Laobian.Lib.Option;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Laobian.Lib.Repository
{
    public class ReadRepository : GitFileRepository, IReadRepository
    {
        private const string FileExt = ".json";

        private readonly LaobianOption _option;
        private readonly SemaphoreSlim _semaphoreSlim;

        public ReadRepository(IOptions<LaobianOption> option)
        {
            _option = option.Value;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async IAsyncEnumerable<ReadItem> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            try
            {
                string baseDir = GetBaseDir();
                foreach (string item in await ReadAllAsync(baseDir, $"*{FileExt}", cancellationToken))
                {
                    IEnumerable<ReadItem> items = JsonHelper.Deserialize<IEnumerable<ReadItem>>(await File.ReadAllTextAsync(item, cancellationToken));
                    foreach (ReadItem r in items)
                    {
                        yield return r;
                    }
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task AddAsync(ReadItem item, CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            try
            {
                string baseDir = GetBaseDir();
                if (item.CreateTime == default)
                {
                    item.CreateTime = DateTime.Now;
                }

                if (string.IsNullOrEmpty(item.BookName))
                {
                    throw new Exception("Missing book name.");
                }

                item.Id = StringHelper.Random();
                item.LastUpdateTime = DateTime.Now;

                List<ReadItem> items = new();
                string path = Path.Combine(baseDir, $"{item.CreateTime.Year}{FileExt}");
                if (File.Exists(path))
                {
                    items = JsonHelper.Deserialize<List<ReadItem>>(await File.ReadAllTextAsync(path, cancellationToken));
                }

                items.Add(item);
                await WriteAsync(path, JsonHelper.Serialize(items, true), cancellationToken);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _ = _semaphoreSlim.Release();
                }
            }
        }

        public async Task UpdateAsync(ReadItem item, CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();

            try
            {
                string baseDir = GetBaseDir();
                if (string.IsNullOrEmpty(item.Id))
                {
                    throw new Exception("Missing book id.");
                }

                if (string.IsNullOrEmpty(item.BookName))
                {
                    throw new Exception("Missing book name.");
                }

                string path = Path.Combine(baseDir, $"{item.CreateTime.Year}{FileExt}");
                if (!File.Exists(path))
                {
                    throw new Exception($"Path {path} not exist.");
                }

                List<ReadItem> items = JsonHelper.Deserialize<List<ReadItem>>(await File.ReadAllTextAsync(path, cancellationToken));
                ReadItem oldItem = items.FirstOrDefault(x => x.Id == item.Id);
                if (oldItem == null)
                {
                    throw new Exception($"Book with id {item.Id} not exist.");
                }

                oldItem.BookName = item.BookName;
                oldItem.Author = item.Author;
                oldItem.AuthorCountry = item.AuthorCountry;
                oldItem.Comment = item.Comment;
                oldItem.Grade = item.Grade;
                oldItem.LastUpdateTime = DateTime.Now;
                oldItem.PostCommentId = item.PostCommentId;
                oldItem.PublishDate = item.PublishDate;
                oldItem.PublisherName = item.PublisherName;
                oldItem.Translator = item.Translator;
                oldItem.IsPublic = item.IsPublic;

                await WriteAsync(path, JsonHelper.Serialize(items, true), cancellationToken);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _ = _semaphoreSlim.Release();
                }
            }
        }

        private string GetBaseDir()
        {
            string path = Path.Combine(_option.AssetLocation, "asset", "read");
            _ = Directory.CreateDirectory(path);
            return path;
        }
    }
}
