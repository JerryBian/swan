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

        public FileObjectStore(IOptions<SwanOption> option, string path, string filter)
        {
            _dir = Path.Combine(option.Value.AssetLocation, Constants.Asset.BaseDir, path);
            _ = Directory.CreateDirectory(_dir);

            _filter = filter;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
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

                    await WriteAsync(path, obj);
                }

                return obj;
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        public async Task<T> UpdateAsync(T obj, bool coreUpdate = true)
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
                        obj.LastUpdateTime = coreUpdate ? DateTime.Now : oldObj.LastUpdateTime;

                        _ = item.Remove(oldObj);
                        item.Add(obj);

                        if (IsStoredAsArray())
                        {
                            await WriteAsync(path, item.OrderByDescending(x => x.CreateTime));
                        }
                        else
                        {
                            await WriteAsync(path, obj);
                        }

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

        public async Task DeleteAsync(Predicate<T> filter)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                List<List<T>> objs = await ReadAllAsync();
                foreach (List<T> item in objs)
                {
                    string fileName = item.First().GetFileName();
                    string path = Path.Combine(_dir, fileName);
                    List<T> targetObjs = item.Where(x => filter(x)).ToList();
                    IEnumerable<T> remainingObjs = targetObjs.Except(targetObjs);
                    if (!remainingObjs.Any())
                    {
                        // No Element: Delete file
                        File.Delete(fileName);
                        continue;
                    }

                    if (IsStoredAsArray())
                    {
                        await WriteAsync(path, remainingObjs.OrderByDescending(x => x.CreateTime));
                    }

                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        private async Task<List<List<T>>> ReadAllAsync()
        {
            List<List<T>> result = new();
            foreach (string file in Directory.EnumerateFiles(_dir, _filter, SearchOption.TopDirectoryOnly))
            {
                string content = await File.ReadAllTextAsync(file, Encoding.UTF8);
                result.Add(!IsStoredAsArray() ? new List<T> { JsonHelper.Deserialize<T>(content) } : JsonHelper.Deserialize<List<T>>(content));
            }

            return result;
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
