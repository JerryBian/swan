using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository;

public class GitFileRepository : IFileRepository
{
    private readonly AutoResetEvent _autoResetEvent;

    public GitFileRepository()
    {
        _autoResetEvent = new AutoResetEvent(true);
    }

    public GitFileRepository(IOptions<ApiOptions> options)
    {
        _autoResetEvent = new AutoResetEvent(true);
        BasePath = Path.Combine(options.Value.AssetLocation, Constants.AssetDbFolder);
    }

    public string BasePath { get; set; }

    public virtual async Task AppendLineAsync(string path, string content,
        CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            var fullPath = Path.Combine(BasePath, path);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.AppendAllLinesAsync(fullPath, new[] {content}, Encoding.UTF8, cancellationToken);
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public virtual async Task WriteAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            var fullPath = Path.Combine(BasePath, path);
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8, cancellationToken);
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public virtual async Task<IEnumerable<string>> SearchFilesAsync(string pattern, string relativePath = null, bool topDirectoryOnly = false,
        CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            var searchPath = string.IsNullOrEmpty(relativePath) ? BasePath : Path.Combine(BasePath, relativePath);
            var dir = Path.GetDirectoryName(pattern);
            if (!string.IsNullOrEmpty(dir))
            {
                searchPath = Path.Combine(searchPath, dir);
                pattern = Path.GetFileName(pattern);
            }

            if (!Directory.Exists(searchPath))
            {
                return Enumerable.Empty<string>();
            }

            return await Task.FromResult(Directory.EnumerateFiles(searchPath, pattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories)
                .Select(x => Path.GetRelativePath(BasePath, x)));
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public async Task<IEnumerable<string>> SearchDirectoriesAsync(string pattern, string relativePath = null, bool topDirectoryOnly = false,
        CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            var searchPath = string.IsNullOrEmpty(relativePath) ? BasePath : Path.Combine(BasePath, relativePath);
            var dir = Path.GetDirectoryName(pattern);
            if (!string.IsNullOrEmpty(dir))
            {
                searchPath = Path.Combine(searchPath, dir);
                pattern = Path.GetFileName(pattern);
            }

            if (!Directory.Exists(searchPath))
            {
                return Enumerable.Empty<string>();
            }

            return await Task.FromResult(Directory.EnumerateDirectories(searchPath, pattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories)
                .Select(x => Path.GetRelativePath(BasePath, x)));
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public virtual async Task<string> ReadAsync(string path, CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            var fullPath = Path.Combine(BasePath, path);
            if (!File.Exists(fullPath))
            {
                throw new Exception($"Path not found: {fullPath}");
            }

            return await File.ReadAllTextAsync(fullPath, Encoding.UTF8, cancellationToken);
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public virtual async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            var fullPath = Path.Combine(BasePath, path);
            if (!File.Exists(fullPath))
            {
                throw new Exception($"Path not found: {fullPath}");
            }

            File.Delete(fullPath);
            await Task.CompletedTask;
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public virtual async Task RenameAsync(string oldPath, string newPath, CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            oldPath = Path.Combine(BasePath, oldPath);
            var oldPathDir = Path.GetDirectoryName(oldPath);
            if (!string.IsNullOrEmpty(oldPathDir))
            {
                Directory.CreateDirectory(oldPathDir);
            }

            newPath = Path.Combine(BasePath, newPath);
            var newPathDir = Path.GetDirectoryName(newPath);
            if (!string.IsNullOrEmpty(newPathDir))
            {
                Directory.CreateDirectory(newPathDir);
            }

            File.Move(oldPath, newPath, true);
            await Task.CompletedTask;
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            path = Path.Combine(BasePath, path);
            return await Task.FromResult(File.Exists(path));
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public async Task AddFileAsync(string path, byte[] content, CancellationToken cancellationToken = default)
    {
        _autoResetEvent.WaitOne();
        try
        {
            EnsureBasePath();
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllBytesAsync(Path.Combine(BasePath, path), content, cancellationToken);
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    public async Task<long> GetFileSizeAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!await FileExistsAsync(path, cancellationToken))
        {
            throw new Exception($"File not exist: {path}");
        }

        _autoResetEvent.WaitOne();
        try
        {
            path = Path.Combine(BasePath, path);
            return new FileInfo(path).Length;
        }
        finally
        {
            _autoResetEvent.Set();
        }
    }

    private void EnsureBasePath()
    {
        if (string.IsNullOrEmpty(BasePath))
        {
            throw new Exception("Base path is not set.");
        }

        Directory.CreateDirectory(BasePath);
    }
}