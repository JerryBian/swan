using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog
{
    public interface IBlogService
    {
        Task<List<BlogPost>> GetPostsAsync(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        Task<BlogPost> GetPostAsync(int year, int month, string link, bool onlyPublic = true);

        Task<List<BlogCategory>> GetCategoriesAsync(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        Task<List<BlogTag>> GetTagsAsync(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        Task<string> GetAboutHtmlAsync();

        Task<List<BlogArchive>> GetArchivesAsync(bool onlyPublic = true, bool publishTimeDesc = true,
            bool toppingPostsFirst = true);

        Task ReloadLocalAssetsAsync(bool clone = true, bool updateTemplate = true);

        Task UpdateRemoteStoreAsync();
    }
}
