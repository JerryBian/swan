using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Store;
using Swan.Lib.Model;

namespace Swan.Core.Service
{
    public class ReadService : IReadService
    {
        private readonly IFileObjectStore<ReadObject> _readObjectStore;

        public ReadService(IFileObjectStore<ReadObject> readObjectStore)
        {
            _readObjectStore = readObjectStore;
        }

        public async Task<List<ReadModel>> GetAllAsync()
        {
            var result = new List<ReadModel>();
            var readObjs = await _readObjectStore.GetAllAsync();
            foreach(var obj in readObjs)
            {
                var readModel = new ReadModel(obj);
                List<string> metadata = new();
                string author = obj.Author;
                if (!string.IsNullOrEmpty(author))
                {
                    if (!string.IsNullOrEmpty(obj.AuthorCountry))
                    {
                        author = $"{author}({obj.AuthorCountry})";
                    }

                    metadata.Add(author);
                }

                if (!string.IsNullOrEmpty(obj.Translator))
                {
                    metadata.Add($"{obj.Translator}(译)");
                }

                if (!string.IsNullOrEmpty(obj.PublisherName))
                {
                    metadata.Add(obj.PublisherName);
                }

                if (obj.PublishDate != default)
                {
                    metadata.Add(obj.PublishDate.ToDate());
                }

                readModel.Metadata = string.Join(" / ", metadata);
                if(!string.IsNullOrEmpty(obj.Comment))
                {
                    readModel.CommentHtml = MarkdownHelper.ToHtml(obj.Comment);
                }
                
                result.Add(readModel);
            }

            return result;
        }

        public async Task<ReadModel> GetAsync(string id)
        {
            var objs = await _readObjectStore.GetAllAsync();
            var obj = objs.FirstOrDefault(x => x.Id == id);
            if(obj == null)
            {
                return null;
            }

            return new ReadModel(obj);
        }

        public async Task AddAsync(ReadObject item)
        {
            await _readObjectStore.AddAsync(item);
        }

        public async Task UpdateAsync(ReadObject item)
        {
            await _readObjectStore.UpdateAsync(item);
        }
    }
}
