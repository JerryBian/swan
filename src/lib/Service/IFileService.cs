namespace Swan.Lib.Service
{
    public interface IFileService
    {
        Task<string> AddAsync(string fileName, byte[] content);
    }
}
