using System.IO;
using Laobian.Api.Source;
using Laobian.Share;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Repository
{
    public class DiaryFileRepository : FileSourceBase, IDiaryFileRepository
    {
        public DiaryFileRepository(IOptions<ApiOptions> options)
        {
            var assetPath = Path.Combine(options.Value.AssetLocation, Constants.AssetDbFolder);
            BasePath = Path.Combine(assetPath, Constants.AssetDbDiaryFolder);
        }
    }
}
