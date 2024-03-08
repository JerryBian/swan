namespace Swan.Core.Store
{
    public interface ISwanGitFolder
    {
        Task SaveAsync();

        Task StartAsync();

        Task StopAsync();

        Task WriteAsync(string path, byte[] content);
    }
}