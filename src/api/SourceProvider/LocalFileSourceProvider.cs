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
        private const string PostFileExtension = "*.md";
        private const string PostMetadataExtension = "*.json";
        private readonly ApiOption _apiOption;
        private string _accessLocation;
        private string _postLocation;
        private string _postMetadataLocation;
        private string _tagLocation;

        public LocalFileSourceProvider(IOptions<ApiOption> apiConfig)
        {
            _apiOption = apiConfig.Value;
        }

        public virtual async Task LoadAsync(bool init = true, CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(_apiOption.GetBlogPostLocation());
            Directory.CreateDirectory(_apiOption.GetDbLocation());
            Directory.CreateDirectory(_apiOption.GetBlogFileLocation());

            var fileFolderInBlogPostLocation = Path.Combine(_apiOption.GetBlogPostLocation(), "file");
            Directory.CreateDirectory(fileFolderInBlogPostLocation);

            var metadataLocation = Path.Combine(_apiOption.GetDbLocation(), "metadata");
            Directory.CreateDirectory(metadataLocation);

            _postMetadataLocation = Path.Combine(metadataLocation, "post.json");
            _tagLocation = Path.Combine(metadataLocation, "tag.json");

            _postLocation = Path.Combine(_apiOption.GetBlogPostLocation(), "post");
            Directory.CreateDirectory(_postLocation);

            _accessLocation = Path.Combine(_apiOption.GetDbLocation(), "access");
            Directory.CreateDirectory(_accessLocation);

            if (init)
            {
                var di = new DirectoryInfo(_apiOption.GetBlogFileLocation());
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                CopyFilesRecursively(new DirectoryInfo(fileFolderInBlogPostLocation),
                    new DirectoryInfo(_apiOption.GetBlogFileLocation()));
            }

            await Task.CompletedTask;
        }

        public virtual async Task<IDictionary<string, string>> GetPostsAsync(
            CancellationToken cancellationToken = default)
        {
            var posts = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(_postLocation, PostFileExtension,
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

        public virtual async Task<IDictionary<string, string>> GetPostAccessAsync(
            CancellationToken cancellationToken = default)
        {
            var access = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var file in Directory.EnumerateFiles(_accessLocation, PostMetadataExtension,
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


        public virtual async Task PersistentAsync(CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
        }

        private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }
        }
    }
}