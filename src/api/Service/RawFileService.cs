using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Service;

public class RawFileService : IRawFileService
{
    private readonly ApiOptions _options;
    private readonly IRawFileRepository _rawFileRepository;

    public RawFileService(IOptions<ApiOptions> options, IRawFileRepository rawFileRepository)
    {
        _options = options.Value;
        _rawFileRepository = rawFileRepository;
    }

    public async Task<string> AddRawFileAsync(string fileName, byte[] content,
        CancellationToken cancellationToken = default)
    {
        var folderName = DateTime.Now.Year.ToString("D4");
        var path = Path.Combine(folderName, fileName);
        await _rawFileRepository.AddFileAsync(path, content, cancellationToken);
        return $"{_options.FileRemoteEndpoint}/{folderName}/{fileName}";
    }
}