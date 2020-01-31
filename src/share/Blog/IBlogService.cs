using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog
{
    public interface IBlogService
    {
        BlogPostAccess GetPostAccess();

        List<BlogPost> GetPosts(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true);

        BlogPost GetPost(int year, int month, string link, bool onlyPublic = true);

        List<BlogCategory> GetCategories(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        List<BlogTag> GetTags(bool onlyPublic = true, bool publishTimeDesc = true, bool toppingPostsFirst = true);

        string GetAboutHtml();

        void NewPostAccess(BlogPost post);

        List<BlogArchive> GetArchives(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        Task InitAsync(bool clone = true);

        Task GitHookAsync(List<string> postLinks);

        Task UpdateGitHubAsync();
    }
}