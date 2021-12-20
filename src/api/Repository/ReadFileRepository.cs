using System.IO;
using Laobian.Api.Source;
using Laobian.Share;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository
{
    public class ReadFileRepository : FileSourceBase, IReadFileRepository
    {
        public ReadFileRepository(IOptions<ApiOptions> options)
        {
            var assetPath = Path.Combine(options.Value.AssetLocation, Constants.AssetDbFolder);
            BasePath = Path.Combine(assetPath, Constants.AssetDbReadFolder);
        }
    }
}
