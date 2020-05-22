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

        Task PullFromGitHubAsync();

        Task<string> ParseAssetsToObjectsAsync();

        Task SerializeAssetsToFilesAsync();

        Task PushToGitHubAsync(string message);

        void MergePosts(List<BlogPost> oldPosts);

        void UpdatePosts(IEnumerable<string> postLinks);
    }
}