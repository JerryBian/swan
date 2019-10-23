using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine.Model;

namespace Laobian.Share.BlogEngine
{
    public interface IBlogService
    {
        BlogPost GetPost(int year, int month, string link);

        List<BlogPost> GetPosts(bool filterPublish, ref int page, out int totalPages);

        List<BlogPost> GetPublishedPosts();

        List<BlogPost> GetPosts();

        List<BlogCategory> GetCategories();

        List<BlogTag> GetTags();

        Task UpdateCloudAssetsAsync();

        Task UpdateMemoryAssetsAsync(bool cloneFirst = true);

        string GetAboutHtml(RequestLang lang = RequestLang.Chinese);
    }
}
