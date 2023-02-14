using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Swan.Core.Helper;
using Swan.Core.Model.Object;
using Swan.Core.Option;
using System.Text;

namespace Swan.Core.Store
{
    public class FileObjectStore<T>  : IFileObjectStore<T> where T : FileObjectBase
    {
        private readonly string _dir;
        private readonly string _filter;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly List<List<T>> _cachedObjects;

        public FileObjectStore(IOptions<SwanOption> option, string path, string filter)
        {
            _dir = Path.Combine(option.Value.AssetLocation, Constants.FolderAsset, path);
            Directory.CreateDirectory(_dir);

            _filter = filter;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _cachedObjects = new List<List<T>>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var items = await ReadAllAsync();
                return items.SelectMany(x => x);
            }
            finally
            {
                _semaphoreSlim.Release();
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

                var fileName = obj.GetFileName();
                var path = Path.Combine(_dir, fileName);

                if (obj.CreateTime == default)
                {
                    obj.CreateTime = DateTime.Now;
                }

                if (obj.LastUpdateTime == default)
                {
                    obj.LastUpdateTime = DateTime.Now;
                }

                var objs = await ReadAllAsync();
                var arrayObjs = new List<T>();
                foreach (var item in objs)
                {
                    var found = false;
                    foreach (var item2 in item)
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
                var path = Path.Combine(_dir, fileName);
                if (!File.Exists(path))
                {
                    throw new Exception($"File not exists: {path}");
                }

                var found = false;
                var objs = await ReadAllAsync();
                foreach (var item in objs)
                {
                    T oldObj = null;
                    foreach (var item2 in item)
                    {
                        if (item2.Id == obj.Id)
                        {
                            var item2FileName = item2.GetFileName();
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

                        item.Remove(oldObj);
                        item.Add(obj);
                        found = true;
                        break;
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

        public async Task DeleteAsync(string id)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var objs = await ReadAllAsync();
                var deleted = false;
                foreach (var item in objs)
                {
                    T obj = null;
                    foreach (var item2 in item)
                    {
                        if (item2.Id == id)
                        {
                            obj = item2;
                            break;
                        }
                    }

                    if (obj != null)
                    {
                        var file = obj.GetFileName();
                        var path = Path.Combine(_dir, file);
                        if (IsStoredAsArray())
                        {
                            item.Remove(obj);
                            if(item.Any())
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

                if(!deleted)
                {
                    throw new Exception($"Id not found: {id}");
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<List<List<T>>> ReadAllAsync()
        {
            if (!_cachedObjects.Any())
            {
                foreach (var file in Directory.EnumerateFiles(_dir, _filter, SearchOption.TopDirectoryOnly))
                {
                    var content = await File.ReadAllTextAsync(file, Encoding.UTF8);
                    List<T> obj;
                    if (!IsStoredAsArray())
                    {
                        obj = new List<T> { JsonHelper.Deserialize<T>(content) };
                    }
                    else
                    {
                        obj = JsonHelper.Deserialize<List<T>>(content);
                    }

                    _cachedObjects.Add(obj);
                }
            }

            return _cachedObjects;
        }

        private async Task WriteAsync(string file, object obj)
        {
            var path = Path.Combine(_dir, file);
            await File.WriteAllTextAsync(path, JsonHelper.Serialize(obj), Encoding.UTF8);
        }

        private bool IsStoredAsArray()
        {
            return !typeof(ISingleObject).IsAssignableFrom(typeof(T));
        }
    }
}
