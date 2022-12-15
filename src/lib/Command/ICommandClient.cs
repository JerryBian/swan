namespace Swan.Lib.Command
{
    public interface ICommandClient
    {
        Task<string> RunAsync(string command, CancellationToken cancellationToken = default);
    }
}
