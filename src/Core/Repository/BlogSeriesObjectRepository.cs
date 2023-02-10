using Microsoft.Extensions.Options;
using Swan.Core.Model.Object;
using Swan.Core.Option;

namespace Swan.Core.Repository
{
    public class BlogSeriesObjectRepository : MultipleFileObjectRepository<BlogSeriesObject>, IBlogSeriesObjectRepository
    {
        public BlogSeriesObjectRepository(IOptions<SwanOption> option) : base(Path.Combine(option.Value.AssetLocation, Constants.FolderAsset, Constants.BlogSeriesPath))
        {
        }
    }
}
