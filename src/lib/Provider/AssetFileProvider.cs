using Laobian.Lib.Option;
using Microsoft.Extensions.Options;

namespace Laobian.Lib.Provider
{
    public class AssetFileProvider : IAssetFileProvider
    {
        private readonly LaobianOption _option;

        public AssetFileProvider(IOptions<LaobianOption> option)
        {
            _option = option.Value;
        }

        public string LogExtension => ".log";

        public string JsonExtension => ".json";

        public string GetLogBaseDir()
        {
            var dir = Path.Combine(_option.AssetLocation, Constants.FolderAsset, Constants.FolderTemp, Constants.FolderTempLog);
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
