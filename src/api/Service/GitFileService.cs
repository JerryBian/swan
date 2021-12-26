using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Command;
using Laobian.Api.Repository;
using Laobian.Api.Source;
using Laobian.Share;
using Laobian.Share.Extension;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Service
{
    public class GitFileService : IGitFileService
    {
        private readonly ApiOptions _apiOptions;
        private readonly IFileRepository _fileRepository;
        private readonly ICommandClient _commandClient;
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
                var dbStat = new GitFileStat{FolderName = "/"};
                foreach (var file in await _fileRepository.SearchFilesAsync("*", cancellationToken: cancellationToken))
                {
                    dbStat.FileCount++;
                    dbStat.FolderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
                }

                dbStat.SubFolderCount =
                    (await _fileRepository.SearchDirectoriesAsync("*", topDirectoryOnly: true,
                        cancellationToken: cancellationToken)).Count();
                stats.Add(dbStat);

                var blogStat = new GitFileStat {FolderName = $"/{Constants.AssetDbBlogFolder}"};
                foreach (var file in await _fileRepository.SearchFilesAsync("*", Constants.AssetDbBlogFolder, cancellationToken: cancellationToken))
                {
                    blogStat.FileCount++;
                    blogStat.FolderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
                }

                blogStat.SubFolderCount =
                    (await _fileRepository.SearchDirectoriesAsync("*", Constants.AssetDbBlogFolder, topDirectoryOnly: true,
                        cancellationToken: cancellationToken)).Count();
                stats.Add(blogStat);

                var diaryStat = new GitFileStat { FolderName = $"/{Constants.AssetDbDiaryFolder}" };
                foreach (var file in await _fileRepository.SearchFilesAsync("*", Constants.AssetDbDiaryFolder, cancellationToken: cancellationToken))
                {
                    diaryStat.FileCount++;
                    diaryStat.FolderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
                }

                diaryStat.SubFolderCount =
                    (await _fileRepository.SearchDirectoriesAsync("*", Constants.AssetDbDiaryFolder, topDirectoryOnly: true,
                        cancellationToken: cancellationToken)).Count();
                stats.Add(diaryStat);

                var fileStat = new GitFileStat { FolderName = $"/{Constants.AssetDbFileFolder}" };
                foreach (var file in await _fileRepository.SearchFilesAsync("*", Constants.AssetDbFileFolder, cancellationToken: cancellationToken))
                {
                    fileStat.FileCount++;
                    fileStat.FolderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
                }

                fileStat.SubFolderCount =
                    (await _fileRepository.SearchDirectoriesAsync("*", Constants.AssetDbFileFolder, topDirectoryOnly: true,
                        cancellationToken: cancellationToken)).Count();
                stats.Add(fileStat);

                var logStat = new GitFileStat { FolderName = $"/{Constants.AssetDbLogFolder}" };
                foreach (var file in await _fileRepository.SearchFilesAsync("*", Constants.AssetDbLogFolder, cancellationToken: cancellationToken))
                {
                    logStat.FileCount++;
                    logStat.FolderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
                }

                logStat.SubFolderCount =
                    (await _fileRepository.SearchDirectoriesAsync("*", Constants.AssetDbLogFolder, topDirectoryOnly: true,
                        cancellationToken: cancellationToken)).Count();
                stats.Add(logStat);

                var noteStat = new GitFileStat { FolderName = $"/{Constants.AssetDbNoteFolder}" };
                foreach (var file in await _fileRepository.SearchFilesAsync("*", Constants.AssetDbNoteFolder, cancellationToken: cancellationToken))
                {
                    noteStat.FileCount++;
                    noteStat.FolderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
                }

                noteStat.SubFolderCount =
                    (await _fileRepository.SearchDirectoriesAsync("*", Constants.AssetDbNoteFolder, topDirectoryOnly: true,
                        cancellationToken: cancellationToken)).Count();
                stats.Add(noteStat);

                var readStat = new GitFileStat { FolderName = $"/{Constants.AssetDbReadFolder}" };
                foreach (var file in await _fileRepository.SearchFilesAsync("*", Constants.AssetDbReadFolder, cancellationToken: cancellationToken))
                {
                    readStat.FileCount++;
                    readStat.FolderSize += await _fileRepository.GetFileSizeAsync(file, cancellationToken);
                }

                readStat.SubFolderCount =
                    (await _fileRepository.SearchDirectoriesAsync("*", Constants.AssetDbReadFolder, topDirectoryOnly: true,
                        cancellationToken: cancellationToken)).Count();
                stats.Add(readStat);
            }

            return stats;
        }
    }
}
