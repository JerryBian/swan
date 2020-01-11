using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Asset
{
    public interface IBlogAssetManager
    {
        List<BlogPost> GetAllPosts();

        List<BlogCategory> GetAllCategories();

        List<BlogTag> GetAllTags();

        string GetAboutHtml();

        Task RemoteGitToLocalFileAsync();

        Task<bool> LocalFileToLocalMemoryAsync();

        Task LocalMemoryToLocalFileAsync();

        Task LocalFileToRemoteGitAsync();
    }
}