using Swan.Core.Helper;
using Swan.Core.Model.Object;

namespace Swan.Core.Repository
{
    public abstract class SingleFileObjectRepository<T> where T : FileObjectBase
    {
        private readonly string _basePath;
        private readonly string _fileFilter;
        private readonly SemaphoreSlim _semaphoreSlim;

        protected SingleFileObjectRepository(string basePath, string fileFilter = Constants.JsonFileFilter) 
        {
            _basePath = basePath;
            Directory.CreateDirectory(_basePath);

            _fileFilter = fileFilter;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<T> GetAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var objs = await GetAllObjectsAsync();
                return objs.Values.FirstOrDefault(x => x.Id == id);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var objs = await GetAllObjectsAsync();
                return objs.Values.ToList();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<T> CreateAsync(T obj)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(obj.Id))
                {
                    obj.Id = StringHelper.Random();
                }
                else
                {
                    var objs = await GetAllObjectsAsync();
                    foreach(var item in objs)
                    {
                        if(item.Value.Id == obj.Id)
                        {
                            throw new Exception($"Id already exists. Path: {item.Key}, Id: {obj.Id}");
                        }
                    }
                }

                var fileName = obj.GetFileName();
                var path = Path.Combine(_basePath, fileName);
                if (File.Exists(path))
                {
                    throw new Exception($"File already exists: {path}");
                }

                if(obj.CreateTime == default)
                {
                    obj.CreateTime = DateTime.Now;
                }

                if(obj.LastUpdateTime == default)
                {
                    obj.LastUpdateTime = DateTime.Now;
                }

                await File.WriteAllTextAsync(path, JsonHelper.Serialize(obj));
                return obj;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<T> UpdateAsync(T obj)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(obj.Id))
                {
                    throw new Exception("Missing id.");
                }

                var fileName = obj.GetFileName();
                var path = Path.Combine(_basePath, fileName);
                if (!File.Exists(path))
                {
                    throw new Exception($"File not exists: {path}");
                }

                var oldObj = await GetObjectAsync(path);
                if(oldObj == null || oldObj.Id != obj.Id)
                {
                    throw new Exception($"Mismatch id found in file: {path}. Old: {oldObj.Id}, New: {obj.Id}");
                }

                obj.CreateTime = oldObj.CreateTime;
                obj.LastUpdateTime = DateTime.Now;
                await File.WriteAllTextAsync(path, JsonHelper.Serialize(obj));

                return obj;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var found = false;
                var objs = await GetAllObjectsAsync();
                foreach(var item in objs)
                {
                    if(item.Value.Id == id)
                    {
                        File.Delete(item.Key);
                        break;
                    }
                }

                if(!found)
                {
                    throw new Exception($"Failed to delete, id not found: {id}");
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<T> GetObjectAsync(string path)
        {
            var content = await File.ReadAllTextAsync(path);
            return JsonHelper.Deserialize<T>(content);
        }

        private async Task<Dictionary<string, T>> GetAllObjectsAsync()
        {
            var result = new Dictionary<string, T>();
            foreach (var file in Directory.EnumerateFiles(_basePath, _fileFilter))
            {
                result.Add(file, await GetObjectAsync(file));
            }

            return result;
        }
    }
}
