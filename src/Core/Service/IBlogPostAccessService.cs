namespace Swan.Core.Service
{
    public interface IBlogPostAccessService
    {
        Task AddAsync(string postId, string ipAddress);
    }
}