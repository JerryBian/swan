using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Laobian.Api.SourceProvider
{
    public class LocalFileSourceProvider : ISourceProvider
    {
        private const string MarkdownExtension = "*.md";
        private const string JsonExtension = "*.json";
        private readonly ApiOption _apiOption;
        private string _accessLocation;
        private string _postLocation;
        private string _postMetadataLocation;
        private string _tagLocation;
        private string _readLocation;

        public LocalFileSourceProvider(IOptions<ApiOption> apiConfig)
        {
            _apiOption = apiConfig.Value;
        }

        public virtual async Task LoadAsync(bool init = true, CancellationToken cancellationToken = default)
        {
            _postLocation = _apiOption.GetBlogPostLocation();
            Directory.CreateDirectory(_postLocation);

            var metadataLocation = _apiOption.GetBlogMetadataLocation();
            Directory.CreateDirectory(metadataLocation);

            _postMetadataLocation = Path.Combine(metadataLocation, "post.json");
            _tagLocation = Path.Combine(metadataLocation, "tag.json");

            _accessLocation = _apiOption.GetBlogAccessLocation();
            Directory.CreateDirectory(_accessLocation);

            _readLocation = _apiOption.GetReadLocation();
            Directory.CreateDirectory(_readLocation);
            await Task.CompletedTask;
        }

        public virtual async Task<IDictionary<string, string>> GetPostsAsync(
            CancellationToken cancellationToken = default)
        {
            var posts = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(_postLocation, MarkdownExtension,
                SearchOption.AllDirectories))
            {
                var postLink = Path.GetFileNameWithoutExtension(file);
                if (posts.ContainsKey(postLink))
                {
                    throw new Exception($"Duplicate post link found: {postLink}");
                }

                posts.Add(postLink, await File.ReadAllTextAsync(file, cancellationToken));
            }

            return posts;
        }

        public virtual async Task<string> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            if (File.Exists(_tagLocation))
            {
                return await File.ReadAllTextAsync(_tagLocation, Encoding.UTF8, cancellationToken);
            }

            return null;
        }

        public virtual async Task SaveTagsAsync(string tags, CancellationToken cancellationToken = default)
        {
            await File.WriteAllTextAsync(_tagLocation, tags, Encoding.UTF8, cancellationToken);
        }

        public virtual async Task<string> GetPostMetadataAsync(CancellationToken cancellationToken = default)
        {
            if (File.Exists(_postMetadataLocation))
            {
                return await File.ReadAllTextAsync(_postMetadataLocation, Encoding.UTF8, cancellationToken);
            }

            return null;
        }

        public virtual async Task SavePostMetadataAsync(string metadata, CancellationToken cancellationToken = default)
        {
            await File.WriteAllTextAsync(_postMetadataLocation, metadata, Encoding.UTF8, cancellationToken);
        }

        public virtual async Task<IDictionary<int, string>> GetReadItemsAsync(
            CancellationToken cancellationToken = default)
        {
            var items = new Dictionary<int, string>();
            foreach (var file in Directory.EnumerateFiles(_readLocation, JsonExtension,
                SearchOption.TopDirectoryOnly))
            {
                items.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(file)), await File.ReadAllTextAsync(file, Encoding.UTF8, cancellationToken));
            }

            return items;
        }

        public virtual async Task<IDictionary<string, string>> GetPostAccessAsync(
            CancellationToken cancellationToken = default)
        {
            var access = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(_accessLocation, JsonExtension,
                SearchOption.TopDirectoryOnly))
            {
                var postLink = Path.GetFileNameWithoutExtension(file);
                if (access.ContainsKey(postLink))
                {
                    throw new Exception($"Duplicate post link found: {postLink}");
                }

                access.Add(postLink, await File.ReadAllTextAsync(file, Encoding.UTF8, cancellationToken));
            }

            return access;
        }

        public virtual async Task SavePostAccessAsync(IDictionary<string, string> postAccess,
            CancellationToken cancellationToken = default)
        {
            if (postAccess == null)
            {
                return;
            }

            Directory.Delete(_accessLocation, true);
            Directory.CreateDirectory(_accessLocation);
            foreach (var (key, value) in postAccess)
            {
                var accessFile = Path.Combine(_accessLocation, key + ".json");
                await File.WriteAllTextAsync(accessFile, value, Encoding.UTF8, cancellationToken);
            }
        }

        public virtual async Task SaveReadItemsAsync(IDictionary<int, string> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null)
            {
                return;
            }

            Directory.Delete(_readLocation, true);
            Directory.CreateDirectory(_readLocation);
            foreach (var (key, value) in items)
            {
                var readItemFile = Path.Combine(_readLocation, key + ".json");
                await File.WriteAllTextAsync(readItemFile, value, Encoding.UTF8, cancellationToken);
            }
        }


        public virtual async Task PersistentAsync(string message, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }
    }
}