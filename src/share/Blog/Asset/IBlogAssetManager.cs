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

<<<<<<< HEAD
        Task<bool> PullFromGitHubAsync();
=======
        BlogPostAccess GetPostVisit();

        Task RemoteGitToLocalFileAsync();
>>>>>>> master

        Task<bool> ParseAssetsToObjectsAsync();

        Task<bool> SerializeAssetsToFilesAsync();

        Task<bool> PushToGitHubAsync(string message);

        bool MergePosts(List<BlogPost> oldPosts);

        bool UpdatePosts(IEnumerable<string> postLinks);
    }
}