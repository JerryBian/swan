using Microsoft.Extensions.Options;
using Swan.Core.Model.Object;
using Swan.Core.Option;

namespace Swan.Core.Repository
{
    public class ReadObjectRepository : MultipleFileObjectRepository<ReadObject>, IReadObjectRepository
    {
        public ReadObjectRepository(IOptions<SwanOption> option) : base(Path.Combine(option.Value.AssetLocation, Constants.FolderAsset, Constants.ReadPath))
        {
        }
    }
}
