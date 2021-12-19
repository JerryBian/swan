using System.IO;
using Laobian.Share;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Source
{
    public class BlogFileSource : FileSourceBase, IBlogFileSource
    {
        public BlogFileSource(IOptions<ApiOptions> options)
        {
            var assetPath = Path.Combine(options.Value.AssetLocation, Constants.AssetDbFolder);
            BasePath = Path.Combine(assetPath, Constants.AssetDbBlogFolder);
        }
    }
}
