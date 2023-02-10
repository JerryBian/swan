using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Repository;
using Swan.Lib.Model;

namespace Swan.Core.Service
{
    public class ReadService : IReadService
    {
        private readonly IReadObjectRepository _readObjectRepository;

        public ReadService(IReadObjectRepository readObjectRepository)
        {
            _readObjectRepository = readObjectRepository;
        }

        public async Task<List<ReadModel>> GetAllAsync()
        {
            var result = new List<ReadModel>();
            var readObjs = await _readObjectRepository.GetAllAsync();
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
            var obj = await _readObjectRepository.GetAsync(id);
            if(obj == null)
            {
                return null;
            }

            return new ReadModel(obj);
        }

        public async Task AddAsync(ReadObject item)
        {
            await _readObjectRepository.CreateAsync(item);
        }

        public async Task UpdateAsync(ReadObject item)
        {
            await _readObjectRepository.UpdateAsync(item);
        }
    }
}
