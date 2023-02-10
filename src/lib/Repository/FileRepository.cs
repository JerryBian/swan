using Microsoft.Extensions.Options;
using Swan.Core;
using Swan.Core.Option;

namespace Swan.Lib.Repository
{
    public class FileRepository : BaseRepository, IFileRepository
    {
        private readonly SwanOption _option;

        public FileRepository(IOptions<SwanOption> option)
        {
            _option = option.Value;
        }

        public async Task AddAsync(string subFolder, string fileName, byte[] content)
        {
            string dir = GetBaseDir(subFolder);
            string path = Path.Combine(dir, fileName);
            await File.WriteAllBytesAsync(path, content);
        }

        private string GetBaseDir(string subFolder)
        {
            string dir = Path.Combine(_option.AssetLocation, Constants.FolderAsset, Constants.FolderFile);
            if (!string.IsNullOrEmpty(subFolder))
            {
                dir = Path.Combine(dir, subFolder);
            }
            _ = Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
