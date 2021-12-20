using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Laobian.Api.Source;

public interface IFileSource2
{
    string BasePath { get; set; }

    Task AppendLineAsync(string path, string content, CancellationToken cancellationToken = default);

    Task WriteAsync(string path, string content, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> SearchAsync(string pattern, string relativePath = null,
        CancellationToken cancellationToken = default);

    Task<string> ReadAsync(string path, CancellationToken cancellationToken = default);

    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    Task RenameAsync(string oldPath, string newPath, CancellationToken cancellationToken = default);

    Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);

    Task AddFileAsync(string path, byte[] content, CancellationToken cancellationToken = default);
}