using Laobian.Lib.Repository;

namespace Laobian.Lib.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<string> AddAsync(string fileName, byte[] content)
        {
            string subFolder = DateTime.Now.Year.ToString();
            await _fileRepository.AddAsync(subFolder, fileName, content);
            return $"/{Constants.RouterFile}/{subFolder}/{fileName}";
        }
    }
}
