using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Api.Repository;

public interface IFileRepository
{
    string BasePath { get; set; }

    Task AppendLineAsync(string path, string content, CancellationToken cancellationToken = default);

    Task WriteAsync(string path, string content, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> SearchFilesAsync(string pattern, string relativePath = null, bool topDirectoryOnly = false,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> SearchDirectoriesAsync(string pattern, string relativePath = null, bool topDirectoryOnly = false,
        CancellationToken cancellationToken = default);

    Task<string> ReadAsync(string path, CancellationToken cancellationToken = default);

    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task RenameAsync(string oldPath, string newPath, CancellationToken cancellationToken = default);

    Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);

    Task AddFileAsync(string path, byte[] content, CancellationToken cancellationToken = default);

    Task<long> GetFileSizeAsync(string path, CancellationToken cancellationToken = default);
}