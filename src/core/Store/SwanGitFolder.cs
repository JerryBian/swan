using GitStoreDotnet;
using Microsoft.Extensions.Options;
using Swan.Core.Helper;
using Swan.Core.Model2;
using Swan.Core.Option;

namespace Swan.Core.Store
{
    internal class SwanGitFolder : ISwanGitFolder
    {
        private readonly SwanOption _option;
        private readonly string _baseDir;
        private readonly IGitStore _gitStore;
        private readonly ISwanDatabase _swanDatabase;

        public SwanGitFolder(IOptions<SwanOption> option, IGitStore gitStore, ISwanDatabase swanDatabase)
        {
            _gitStore = gitStore;
            _option = option.Value;
            _baseDir = Path.Combine(_option.DataLocation, "git");
            Directory.CreateDirectory(_baseDir);
            _swanDatabase = swanDatabase;
        }

        public async Task StartAsync()
        {
            if (!_option.SkipGitOperation)
            {
                await _gitStore.PullFromRemoteAsync();
            }
        }

        public async Task SaveAsync()
        {
            // Swan Tag
            var tags = await _swanDatabase.QueryAsync<SwanTag>();
            var tagsJson = JsonHelper.Serialize(tags, true);
            await WriteAsync(Path.Combine(SwanTag.ObjectName, "tag.json"), tagsJson);

            // Swan post
            var posts = await _swanDatabase.QueryAsync<SwanPost>();
            foreach (var post in posts)
            {
                var path = Path.Combine(SwanPost.ObjectName, $"{post.Id}.json");
                var postJson = JsonHelper.Serialize(post, true);
                await WriteAsync(path, postJson);
            }

            // Swan read
            var reads = await _swanDatabase.QueryAsync<SwanRead>();
            var readsJson = JsonHelper.Serialize(reads, true);
            await WriteAsync(Path.Combine(SwanRead.ObjectName, "read.json"), readsJson);

            if (!_option.SkipGitOperation)
            {
                await _gitStore.PushToRemoteAsync("Save periodically.");
            }
        }

        public async Task StopAsync()
        {
            if (!_option.SkipGitOperation)
            {
                await _gitStore.PushToRemoteAsync("Service stopped.");
            }
        }

        private async Task WriteAsync(string path, string content)
        {
            await _gitStore.InsertOrUpdateAsync(path, content);
        }

        public async Task WriteAsync(string path, byte[] content)
        {
            await _gitStore.InsertOrUpdateAsync(path, content);
        }
    }
}
