using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Microsoft.Extensions.Options;

namespace Laobian.Api.SourceProvider
{
    public class LocalFileSourceProvider : ISourceProvider
    {
        private readonly ApiConfig _apiConfig;
        private string _postLocation;
        private string _postMetadataLocation;
        private string _tagLocation;
        private string _commentLocation;
        private string _accessLocation;

        private const string PostFileExtension = "*.md";
        private const string PostMetadataExtension = "*.json";

        public LocalFileSourceProvider(IOptions<ApiConfig> apiConfig)
        {
            _apiConfig = apiConfig.Value;
        }

        public virtual async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_apiConfig.BlogPostLocation))
            {
                throw new LaobianConfigException(_apiConfig.BlogPostLocation);
            }

            if (string.IsNullOrEmpty(_apiConfig.DbLocation))
            {
                throw new LaobianConfigException(_apiConfig.DbLocation);
            }

            Directory.CreateDirectory(_apiConfig.BlogPostLocation);
            Directory.CreateDirectory(_apiConfig.DbLocation);

            var metadataLocation = Path.Combine(_apiConfig.DbLocation, "metadata");
            Directory.CreateDirectory(metadataLocation);

            _postMetadataLocation = Path.Combine(metadataLocation, "post.json");
            _tagLocation = Path.Combine(metadataLocation, "tag.json");

            _postLocation = Path.Combine(_apiConfig.BlogPostLocation, "post");
            Directory.CreateDirectory(_postLocation);

            _accessLocation = Path.Combine(_apiConfig.DbLocation, "access");
            Directory.CreateDirectory(_accessLocation);

            _commentLocation = Path.Combine(_apiConfig.DbLocation, "comment");
            Directory.CreateDirectory(_commentLocation);

            await Task.CompletedTask;
        }

        public virtual async Task<IDictionary<string, string>> GetPostsAsync(CancellationToken cancellationToken = default)
        {
            var posts = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(_postLocation, PostFileExtension, SearchOption.AllDirectories))
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

        public virtual async Task<IDictionary<string, string>> GetPostAccessAsync(CancellationToken cancellationToken = default)
        {
            var access = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(_accessLocation, PostMetadataExtension, SearchOption.TopDirectoryOnly))
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

        public virtual async Task SavePostAccessAsync(IDictionary<string, string> postAccess, CancellationToken cancellationToken = default)
        {
            if (postAccess == null || !postAccess.Any())
            {
                return;
            }

            Directory.Delete(_accessLocation, true);
            Directory.CreateDirectory(_accessLocation);
            foreach (var (key, value) in postAccess)
            {
                var accessFile = Path.Combine(_accessLocation, key + PostMetadataExtension);
                await File.WriteAllTextAsync(accessFile, value, Encoding.UTF8, cancellationToken);
            }
        }

        public virtual async Task<IDictionary<string, string>> GetCommentsAsync(CancellationToken cancellationToken = default)
        {
            var comments = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(_commentLocation, PostMetadataExtension, SearchOption.TopDirectoryOnly))
            {
                var postLink = Path.GetFileNameWithoutExtension(file);
                if (comments.ContainsKey(postLink))
                {
                    throw new Exception($"Duplicate post link found: {postLink}");
                }

                comments.Add(postLink, await File.ReadAllTextAsync(file, Encoding.UTF8, cancellationToken));
            }

            return comments;
        }

        public virtual async Task SaveCommentsAsync(IDictionary<string, string> comments, CancellationToken cancellationToken = default)
        {
            if (comments == null || !comments.Any())
            {
                return;
            }

            Directory.Delete(_commentLocation, true);
            Directory.CreateDirectory(_commentLocation);
            foreach (var (key, value) in comments)
            {
                var commentFile = Path.Combine(_commentLocation, key + PostMetadataExtension);
                await File.WriteAllTextAsync(commentFile, value, Encoding.UTF8, cancellationToken);
            }
        }
    }
}
