using Swan.Core.Extension;
using System.Text;

namespace Swan.Lib.Repository
{
    public abstract class BaseRepository
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public BaseRepository()
        {
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public virtual async Task WriteAsync(string path, string content, CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                string dir = Path.GetDirectoryName(path);
                _ = Directory.CreateDirectory(dir);

                await File.WriteAllTextAsync(path, content, new UTF8Encoding(false), cancellationToken).OkForCancel();
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public virtual async Task WriteAsync(string path, byte[] content, CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                string dir = Path.GetDirectoryName(path);
                _ = Directory.CreateDirectory(dir);

                await File.WriteAllBytesAsync(path, content, cancellationToken).OkForCancel();
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public virtual async Task<IEnumerable<string>> ReadAllAsync(string dirPath, string searchPattern = "*", CancellationToken cancellationToken = default)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            if (cancellationToken.IsCancellationRequested)
            {
                return Enumerable.Empty<string>();
            }

            try
            {
                return !Directory.Exists(dirPath)
                    ? Enumerable.Empty<string>()
                    : Directory.EnumerateFiles(dirPath, searchPattern, SearchOption.AllDirectories);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public virtual async Task<IDictionary<string, List<string>>> ReadItemsAsync(string dirPath, string searchPattern = "*", CancellationToken cancellationToken = default)
        {
            Dictionary<string, List<string>> dict = new();
            await _semaphoreSlim.WaitAsync(cancellationToken).OkForCancel();
            if (cancellationToken.IsCancellationRequested)
            {
                return dict;
            }

            try
            {
                if (!Directory.Exists(dirPath))
                {
                    return dict;
                }

                foreach (string d in Directory.EnumerateDirectories(dirPath, "*", SearchOption.TopDirectoryOnly))
                {
                    dict[d] = new List<string>();
                    foreach (string f in Directory.EnumerateFiles(d, searchPattern, SearchOption.AllDirectories))
                    {
                        dict[d].Add(f);
                    }
                }

                return dict;
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }
    }
}
