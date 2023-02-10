using Microsoft.Extensions.Options;
using Swan.Core.Model.Object;
using Swan.Core.Option;

namespace Swan.Core.Repository
{
    public class BlogTagObjectRepository : MultipleFileObjectRepository<BlogTagObject>, IBlogTagObjectRepository
    {
        public BlogTagObjectRepository(IOptions<SwanOption> option) : base(Path.Combine(option.Value.AssetLocation, Constants.FolderAsset, Constants.BlogTagPath))
        {
        }
    }
}
