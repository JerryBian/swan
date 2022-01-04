using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;
using Laobian.Api.Command;
using Laobian.Api.Repository;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Service;

public class GitFileService : IGitFileService
{
    private readonly ApiOptions _apiOptions;
    private readonly ICommandClient _commandClient;
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<GitFileService> _logger;

    public GitFileService(IOptions<ApiOptions> apiOption, ILogger<GitFileService> logger,
        ICommandClient commandClient, IFileRepository fileRepository)
    {
        _logger = logger;
        _commandClient = commandClient;
        _apiOptions = apiOption.Value;
        _fileRepository = fileRepository;
    }

    public async Task PullAsync(CancellationToken cancellationToken = default)
    {
        if (bool.TryParse(_apiOptions.SkipGitOperations, out var val) && val)
        {
            _logger.LogWarning("Skip git operations due to configuration.");
            return;
        }

        var assetDbFolder = Path.Combine(_apiOptions.AssetLocation, Constants.AssetDbFolder);
        if (Directory.Exists(assetDbFolder))
        {
            Directory.Delete(assetDbFolder, true);
        }

        var retryTimes = 0;
        while (retryTimes <= 3 && !Directory.Exists(assetDbFolder))
        {
            retryTimes++;
            var repoUrl =
                $"https://{_apiOptions.GitHubDbRepoApiToken}@github.com/{_apiOptions.GitHubDbRepoUserName}/{_apiOptions.GitHubDbRepoName}.git";
            var command =
                $"git clone -b {_apiOptions.GitHubDbRepoBranchName} --single-branch {repoUrl} {assetDbFolder}";
            command += $" && cd {assetDbFolder}";
            command += " && git config --local user.name \"API Server\"";
            command += $" && git config --local user.email \"{_apiOptions.AdminEmail}\"";
            if (retryTimes > 1)
            {
                _logger.LogInformation($"Retry: {retryTimes}... starting to pull DB repo.");
            }

            var output = await _commandClient.RunAsync(command, cancellationToken);
            if (retryTimes > 1)
            {
                _logger.LogInformation($"Retry: {retryTimes}, cmd: {command}{Environment.NewLine}");
            }

            _logger.LogInformation(output);
        }

        if (!Directory.Exists(assetDbFolder))
        {
            _logger.LogError("Pull DB repo failed.");
        }
    }

    public async Task PushAsync(string message, CancellationToken cancellationToken = default)
    {
        if (bool.TryParse(_apiOptions.SkipGitOperations, out var val) && val)
        {
            _logger.LogWarning("Skip git operations due to configuration.");
            return;
        }

        var assetDbFolder = Path.Combine(_apiOptions.AssetLocation, Constants.AssetDbFolder);
        if (!Directory.Exists(assetDbFolder))
        {
            _logger.LogWarning($"Push DB repo failed, local dir not exist: {assetDbFolder}.");
            return;
        }

        var commands = new List<string>
        {
            $"cd \"{assetDbFolder}\"", "git add .",
            $"git commit -m \"{message} [{DateTime.Now.ToTime()}]\"", "git push"
        };
        var command =
            $"{string.Join(" && ", commands)}";
        var output = await _commandClient.RunAsync(command, cancellationToken);
        _logger.LogInformation($"cmd: {command}{Environment.NewLine}Output: {output}");
    }

    public async Task<List<GitFileStat>> GetGitFileStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = new List<GitFileStat>();
        var assetDbFolder = _fileRepository.BasePath;
        if (Directory.Exists(assetDbFolder))
        {
            stats.Add(await GetGitFileStateAsync("", cancellationToken));
            stats.Add(await GetGitFileStateAsync(Constants.AssetDbBlogFolder, cancellationToken));
            stats.Add(await GetGitFileStateAsync(Constants.AssetDbDiaryFolder, cancellationToken));
            stats.Add(await GetGitFileStateAsync(Constants.AssetDbFileFolder, cancellationToken));
            stats.Add(await GetGitFileStateAsync(Constants.AssetDbLogFolder, cancellationToken));
            stats.Add(await GetGitFileStateAsync(Constants.AssetDbNoteFolder, cancellationToken));
            stats.Add(await GetGitFileStateAsync(Constants.AssetDbReadFolder, cancellationToken));
        }

        return stats;
    }

    private async Task<GitFileStat> GetGitFileStateAsync(string folder, CancellationToken cancellationToken = default)
    {
        var dbStat = new GitFileStat {FolderName = $"/{folder}"};
        var folderSize = 0L;
        foreach (var file in await _fileRepository.SearchFilesAsync("*", folder, cancellationToken: cancellationToken))
        {
            dbStat.FileCount++;
            folderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
        }

        dbStat.FolderSize = ByteSize.FromBytes(folderSize).ToString();
        dbStat.SubFolderCount =
            (await _fileRepository.SearchDirectoriesAsync("*", folder, true,
                cancellationToken)).Count();
        return dbStat;
    }
}