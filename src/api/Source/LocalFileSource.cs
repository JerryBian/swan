using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Source
{
    public class LocalFileSource : IFileSource
    {
        private readonly LaobianApiOption _apiOption;
        private readonly string _assetDbBlogFolder;
        private readonly string _assetDbFileFolder;
        private readonly string _assetDbLogFolder;
        private readonly string _assetDbReadFolder;
        private readonly ILogger<LocalFileSource> _logger;
        protected readonly ManualResetEventSlim FileLocker;

        public LocalFileSource(IOptions<LaobianApiOption> apiOption, ILogger<LocalFileSource> logger)
        {
            _logger = logger;
            _apiOption = apiOption.Value;

            _assetDbFileFolder = Path.Combine(apiOption.Value.AssetLocation, Constants.AssetDbFolder,
                Constants.AssetDbFileFolder);
            Directory.CreateDirectory(_assetDbFileFolder);

            _assetDbBlogFolder = Path.Combine(apiOption.Value.AssetLocation, Constants.AssetDbFolder,
                Constants.AssetDbBlogFolder);
            Directory.CreateDirectory(_assetDbBlogFolder);

            _assetDbReadFolder = Path.Combine(apiOption.Value.AssetLocation, Constants.AssetDbFolder,
                Constants.AssetDbReadFolder);
            Directory.CreateDirectory(_assetDbReadFolder);

            _assetDbLogFolder = Path.Combine(apiOption.Value.AssetLocation, Constants.AssetDbFolder,
                Constants.AssetDbLogFolder);
            Directory.CreateDirectory(_assetDbLogFolder);
            FileLocker = new ManualResetEventSlim(true);
        }

        public async Task<List<string>> ReadBlogPostsAsync(CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var result = new List<string>();
            var blogPostFolder = Path.Combine(_assetDbBlogFolder, Constants.AssetDbBlogPostFolder);
            if (!Directory.Exists(blogPostFolder))
            {
                return result;
            }

            foreach (var blogPostFile in
                Directory.EnumerateFiles(blogPostFolder, "*.json", SearchOption.AllDirectories))
            {
                result.Add(await File.ReadAllTextAsync(blogPostFile, cancellationToken));
            }

            return result;
        }

        public async Task<string> ReadBlogPostAsync(string postLink, CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var blogPostFolder = Path.Combine(_assetDbBlogFolder, Constants.AssetDbBlogPostFolder);
            if (!Directory.Exists(blogPostFolder))
            {
                return null;
            }

            var posts = Directory.GetFiles(blogPostFolder, $"{postLink}.json", SearchOption.AllDirectories);
            if (posts.Length == 0)
            {
                return null;
            }

            if (posts.Length > 1)
            {
                _logger.LogError($"Duplicate post found: {postLink}. Paths: {string.Join(" <<<>>> ", posts)}");
                return null;
            }

            return await File.ReadAllTextAsync(posts[0], Encoding.UTF8, cancellationToken);
        }

        public async Task WriteBlogPostAsync(int year, string postLink, string content,
            CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var blogPostSubFolder =
                Path.Combine(_assetDbBlogFolder, Constants.AssetDbBlogPostFolder, year.ToString("D4"));
            Directory.CreateDirectory(blogPostSubFolder);
            var blogPostFile = Path.Combine(blogPostSubFolder, $"{postLink}.json");
            await File.WriteAllTextAsync(blogPostFile, content, Encoding.UTF8, cancellationToken);
        }

        public async Task<string> ReadBlogPostAccessAsync(string postLink,
            CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var blogPostAccessFolder = Path.Combine(_assetDbBlogFolder, Constants.AssetDbBlogAccessFolder);
            if (!Directory.Exists(blogPostAccessFolder))
            {
                return null;
            }

            var access = Directory.GetFiles(blogPostAccessFolder, $"{postLink}.json",
                SearchOption.AllDirectories);
            if (access.Length == 0)
            {
                return null;
            }

            if (access.Length > 1)
            {
                _logger.LogError($"Duplicate post access found: {postLink}. Paths: {string.Join(" <<<>>> ", access)}");
                return null;
            }

            return await File.ReadAllTextAsync(access[0], Encoding.UTF8, cancellationToken);
        }

        public async Task WriteBlogPostAccessAsync(int year, string postLink, string content,
            CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var blogPostAccessSubFolder =
                Path.Combine(_assetDbBlogFolder, Constants.AssetDbBlogAccessFolder, year.ToString("D4"));
            Directory.CreateDirectory(blogPostAccessSubFolder);
            var blogPostAccessFile = Path.Combine(blogPostAccessSubFolder, $"{postLink}.json");
            await File.WriteAllTextAsync(blogPostAccessFile, content, Encoding.UTF8, cancellationToken);
        }

        public async Task<string> ReadBlogTagsAsync(CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var blogTagFile = Path.Combine(_assetDbBlogFolder, "tag.json");
            if (!File.Exists(blogTagFile))
            {
                return null;
            }

            return await File.ReadAllTextAsync(blogTagFile, Encoding.UTF8, cancellationToken);
        }

        public async Task WriteBlogTagsAsync(string blogTags, CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var blogTagFile = Path.Combine(_assetDbBlogFolder, "tag.json");
            await File.WriteAllTextAsync(blogTagFile, blogTags, Encoding.UTF8, cancellationToken);
        }

        public async Task<string> ReadLogsAsync(LaobianSite site, DateTime date,
            CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var logFile = Path.Combine(_assetDbLogFolder, site.ToString().ToLowerInvariant(), date.ToString("yyyy"),
                $"{date:yyyy-MM-dd}.log");
            if (!File.Exists(logFile))
            {
                return null;
            }

            return await File.ReadAllTextAsync(logFile, Encoding.UTF8, cancellationToken);
        }

        public async Task AppendLogAsync(LaobianSite site, DateTime date, string log,
            CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var logDir = Path.Combine(_assetDbLogFolder, site.ToString().ToLowerInvariant(), date.ToString("yyyy"));
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, $"{date:yyyy-MM-dd}.log");
            await File.AppendAllLinesAsync(logFile, new List<string> {log}, Encoding.UTF8, cancellationToken);
        }

        public async Task<IDictionary<string, string>> ReadBookItemsAsync(CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in Directory.EnumerateFiles(_assetDbReadFolder, "*.json", SearchOption.TopDirectoryOnly))
            {
                result.Add(Path.GetFileNameWithoutExtension(item),
                    await File.ReadAllTextAsync(item, Encoding.UTF8, cancellationToken));
            }

            return result;
        }

        public async Task<string> ReadBookItemsAsync(int year, CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var bookItemFile = Path.Combine(_assetDbReadFolder, $"{year:D4}.json");
            if (!File.Exists(bookItemFile))
            {
                return null;
            }

            return await File.ReadAllTextAsync(bookItemFile, Encoding.UTF8, cancellationToken);
        }

        public async Task WriteBookItemsAsync(int year, string content, CancellationToken cancellationToken = default)
        {
            FileLocker.Wait(cancellationToken);
            var bookItemFile = Path.Combine(_assetDbReadFolder, $"{year:D4}.json");
            await File.WriteAllTextAsync(bookItemFile, content, Encoding.UTF8, cancellationToken);
        }

        public async Task<string> AddRawFileAsync(string fileName, byte[] content,
            CancellationToken cancellationToken = default)
        {
            var folderName = DateTime.Now.Year.ToString("D4");
            var folder = Path.Combine(_assetDbFileFolder, folderName);
            Directory.CreateDirectory(folder);
            await File.WriteAllBytesAsync(Path.Combine(folder, fileName), content, cancellationToken);
            return $"{_apiOption.FileRemoteEndpoint}/{folderName}/{fileName}";
        }

        public virtual Task FlushAsync(string message)
        {
            return Task.CompletedTask;
        }

        public virtual Task PrepareAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}