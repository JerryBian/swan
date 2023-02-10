using Swan.Core.Helper;
using Swan.Core.Model.Object;
using System.Collections.Immutable;

namespace Swan.Core.Repository
{
    public abstract class MultipleFileObjectRepository<T> where T : FileObjectBase
    {
        private readonly string _basePath;
        private readonly string _fileFilter;
        private readonly SemaphoreSlim _semaphoreSlim;

        protected MultipleFileObjectRepository(string basePath, string fileFilter = Constants.JsonFileFilter) 
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
                return objs.Values.SelectMany(x => x).FirstOrDefault(x => x.Id == id);
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
                return objs.Values.SelectMany(x => x).ToList();
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

                var fileName = obj.GetFileName();
                var path = Path.Combine(_basePath, fileName);

                if (obj.CreateTime == default)
                {
                    obj.CreateTime = DateTime.Now;
                }

                if (obj.LastUpdateTime == default)
                {
                    obj.LastUpdateTime = DateTime.Now;
                }

                var objs = await GetAllObjectsAsync();
                var fileObjs = new List<T>();
                foreach(var item in objs)
                {
                    foreach(var item2 in item.Value)
                    {
                        if(item2.Id == obj.Id)
                        {
                            throw new Exception($"Id already exists. Path: {path}, Id: {obj.Id}");
                        }
                    }

                    if(item.Key == path)
                    {
                        fileObjs = item.Value;
                    }
                }

                fileObjs.Add(obj);
                await File.WriteAllTextAsync(path, JsonHelper.Serialize(fileObjs));
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

                var found = false;
                var objs = await GetAllObjectsAsync();
                foreach(var item in objs)
                {
                    foreach(var item2 in item.Value)
                    {
                        if(item2.Id == obj.Id)
                        {
                            if(!StringHelper.EqualsIgoreCase(Path.GetFullPath(path), Path.GetFullPath(item.Key)))
                            {
                                throw new Exception($"Path mismatch. Old: {item.Key}, New: {path}");
                            }

                            obj.CreateTime = item2.CreateTime;
                            obj.LastUpdateTime = DateTime.Now;

                            var newObjs = item.Value.ToList();
                            newObjs.Remove(item2);
                            newObjs.Add(obj);
                            await File.WriteAllTextAsync(path, JsonHelper.Serialize(newObjs));
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    throw new Exception($"No existing id found: {obj.Id}");
                }

                return obj;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task DeleteAsync(string path)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (!File.Exists(path))
                {
                    throw new Exception($"File not exists: {path}");
                }

                File.Delete(path);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<List<T>> GetObjectsAsync(string path)
        {
            var content = await File.ReadAllTextAsync(path);
            return JsonHelper.Deserialize<List<T>>(content);
        }

        private async Task<Dictionary<string, List<T>>> GetAllObjectsAsync()
        {
            var result = new Dictionary<string, List<T>>();
            foreach (var file in Directory.EnumerateFiles(_basePath, _fileFilter))
            {
                result.Add(file, await GetObjectsAsync(file));
            }

            return result;
        }
    }
}
