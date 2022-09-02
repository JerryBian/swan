using Laobian.Lib.Extension;
using System.Text;

namespace Laobian.Lib.Repository
{
    public abstract class GitFileRepository
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public GitFileRepository()
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

        public virtual async Task AddFileAsync(string path, byte[] content, CancellationToken cancellationToken = default)
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
    }
}
