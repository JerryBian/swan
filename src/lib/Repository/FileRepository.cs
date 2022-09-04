using Laobian.Lib.Option;
using Microsoft.Extensions.Options;

namespace Laobian.Lib.Repository
{
    public class FileRepository : BaseRepository, IFileRepository
    {
        private readonly LaobianOption _option;

        public FileRepository(IOptions<LaobianOption> option)
        {
            _option = option.Value;
        }

        public async Task AddAsync(string subFolder, string fileName, byte[] content)
        {
            var dir = GetBaseDir(subFolder);
            var path = Path.Combine(dir, fileName);
            await File.WriteAllBytesAsync(path, content);
        }

        private string GetBaseDir(string subFolder)
        {
            var dir = Path.Combine(_option.AssetLocation, Constants.FolderAsset, Constants.FolderFile);
            if(!string.IsNullOrEmpty(subFolder))
            {
                dir = Path.Combine(dir, subFolder);
            }
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
