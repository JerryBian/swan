using Microsoft.Extensions.Options;
using Swan.Core.Helper;
using Swan.Core.Model.Object;
using Swan.Core.Option;
using System.Text;

namespace Swan.Core.Store
{
    public class FileObjectStore<T> : IFileObjectStore<T> where T : FileObjectBase
    {
        private readonly string _dir;
        private readonly string _filter;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly List<List<T>> _cachedObjects;

        public FileObjectStore(IOptions<SwanOption> option, string path, string filter)
        {
            _dir = Path.Combine(option.Value.AssetLocation, Constants.Asset.BaseDir, path);
            _ = Directory.CreateDirectory(_dir);

            _filter = filter;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _cachedObjects = new List<List<T>>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                List<List<T>> items = await ReadAllAsync();
                return items.SelectMany(x => x);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task<T> AddAsync(T obj)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(obj.Id))
                {
                    obj.Id = StringHelper.Random();
                }

                string fileName = obj.GetFileName();
                string path = Path.Combine(_dir, fileName);

                if (obj.CreateTime == default)
                {
                    obj.CreateTime = DateTime.Now;
                }

                if (obj.LastUpdateTime == default)
                {
                    obj.LastUpdateTime = DateTime.Now;
                }

                List<List<T>> objs = await ReadAllAsync();
                List<T> arrayObjs = new();
                foreach (List<T> item in objs)
                {
                    bool found = false;
                    foreach (T item2 in item)
                    {
                        if (item2.Id == obj.Id)
                        {
                            throw new Exception($"Id already exists. File: {item2.GetFileName()}, Id: {obj.Id}");
                        }

                        if (item2.GetFileName() == fileName)
                        {
                            found = true;
                        }
                    }

                    if (found)
                    {
                        arrayObjs = item;
                    }
                }

                if (IsStoredAsArray())
                {
                    arrayObjs.Add(obj);
                    await WriteAsync(path, arrayObjs.OrderByDescending(x => x.CreateTime));
                }
                else
                {
                    if (File.Exists(path))
                    {
                        throw new Exception($"File already exists: {path}");
                    }

                    _cachedObjects.Add(new List<T> { obj });
                    await WriteAsync(path, obj);
                }

                return obj;
            }
            finally
            {
                _ = _semaphoreSlim.Release();
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

                string fileName = obj.GetFileName();
                string path = Path.Combine(_dir, fileName);
                if (!File.Exists(path))
                {
                    throw new Exception($"File not exists: {path}");
                }

                bool found = false;
                List<List<T>> objs = await ReadAllAsync();
                foreach (List<T> item in objs)
                {
                    T oldObj = null;
                    foreach (T item2 in item)
                    {
                        if (item2.Id == obj.Id)
                        {
                            string item2FileName = item2.GetFileName();
                            if (item2FileName != fileName)
                            {
                                throw new Exception($"Found same id: {obj.Id} with different file: {item2FileName} vs {fileName}");
                            }

                            oldObj = item2;
                            break;
                        }
                    }

                    if (oldObj != null)
                    {
                        obj.CreateTime = oldObj.CreateTime;
                        obj.LastUpdateTime = DateTime.Now;

                        if (IsStoredAsArray())
                        {
                            await WriteAsync(path, item.OrderByDescending(x => x.CreateTime));
                        }
                        else
                        {
                            await WriteAsync(path, obj);
                        }

                        _ = item.Remove(oldObj);
                        item.Add(obj);
                        found = true;
                        break;
                    }
                }

                return !found ? throw new Exception($"No existing id found: {obj.Id}") : obj;
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                List<List<T>> objs = await ReadAllAsync();
                bool deleted = false;
                foreach (List<T> item in objs)
                {
                    T obj = null;
                    foreach (T item2 in item)
                    {
                        if (item2.Id == id)
                        {
                            obj = item2;
                            break;
                        }
                    }

                    if (obj != null)
                    {
                        string file = obj.GetFileName();
                        string path = Path.Combine(_dir, file);
                        if (IsStoredAsArray())
                        {
                            _ = item.Remove(obj);
                            if (item.Any())
                            {
                                await WriteAsync(path, item.OrderByDescending(x => x.CreateTime));
                            }
                            else
                            {
                                File.Delete(path);
                            }
                        }
                        else
                        {
                            File.Delete(path);
                        }

                        deleted = true;
                        break;
                    }
                }

                if (!deleted)
                {
                    throw new Exception($"Id not found: {id}");
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        private async Task<List<List<T>>> ReadAllAsync()
        {
            if (!_cachedObjects.Any())
            {
                foreach (string file in Directory.EnumerateFiles(_dir, _filter, SearchOption.TopDirectoryOnly))
                {
                    string content = await File.ReadAllTextAsync(file, Encoding.UTF8);
                    List<T> obj = !IsStoredAsArray() ? new List<T> { JsonHelper.Deserialize<T>(content) } : JsonHelper.Deserialize<List<T>>(content);
                    _cachedObjects.Add(obj);
                }
            }

            return _cachedObjects;
        }

        private async Task WriteAsync(string file, object obj)
        {
            string path = Path.Combine(_dir, file);
            await File.WriteAllTextAsync(path, JsonHelper.Serialize(obj), Encoding.UTF8);
        }

        private bool IsStoredAsArray()
        {
            return !typeof(ISingleObject).IsAssignableFrom(typeof(T));
        }
    }
}
