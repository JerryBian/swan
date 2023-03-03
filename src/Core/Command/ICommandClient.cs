namespace Swan.Core.Command
{
    public interface ICommandClient
    {
        Task<string> RunAsync(string command, CancellationToken cancellationToken = default);
    }
}
