using GitStoreDotnet;
using Microsoft.Extensions.Options;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Option;
using Swan.Core.Service;

namespace Swan.Core.Store
{
    internal class SwanGitFolder
    {
        private readonly SwanOption _option;
        private readonly string _baseDir;
        private readonly IGitStore _gitStore;

        public SwanGitFolder(IOptions<SwanOption> option, IGitStore gitStore)
        {
            _gitStore = gitStore;
            _option = option.Value;
            _baseDir = Path.Combine(_option.DataLocation, "git");
            Directory.CreateDirectory(_baseDir);
        }

        public async Task<SwanInternalObject> LoadAsync()
        {
            if (!_option.SkipGitOperation)
            {
                await _gitStore.PullFromRemoteAsync();
            }

            var obj = new SwanInternalObject();
            var readsJson = await _gitStore.GetTextAsync("read/read.json");
            obj.Reads.AddRange(JsonHelper.Deserialize<List<SwanRead>>(readsJson));

            var tagsJson = await _gitStore.GetTextAsync("tag/tag.json");
            obj.Tags.AddRange(JsonHelper.Deserialize<List<SwanTag>>(tagsJson));

            
            return obj;
        }

        public async Task SaveAsync()
        {
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

        public async Task WriteAsync<T>(T obj) where T : ISwanObject
        {
            if(obj == null)
            {
                return;
            }

            await _gitStore.InsertOrUpdateAsync(obj.GitPath, JsonHelper.Serialize(obj));
        }

        public async Task WriteAsync<T>(List<T> objs) where T : ISwanObject
        {
            if(objs == null || objs.Count == 0)
            {
                return;
            }

            await _gitStore.InsertOrUpdateAsync(objs.First().GitPath, JsonHelper.Serialize(objs));
        }

        public async Task WriteAsync(string path, byte[] content)
        {
            await _gitStore.InsertOrUpdateAsync(path, content);
        }
    }
}
