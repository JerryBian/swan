using Microsoft.Extensions.Options;
using Swan.Core.Model.Object;
using Swan.Core.Option;

namespace Swan.Core.Repository
{
    public class BlogPostObjectRepository : SingleFileObjectRepository<BlogPostObject>, IBlogPostObjectRepository
    {
        public BlogPostObjectRepository(IOptions<SwanOption> option) : base(Path.Combine(option.Value.AssetLocation, Constants.FolderAsset, Constants.BlogPostPath))
        {
        }
    }
}
