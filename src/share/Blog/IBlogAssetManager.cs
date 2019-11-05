using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog
{
    public interface IBlogAssetManager
    {
        Task<List<BlogPost>> GetAllPostsAsync();

        Task<List<BlogCategory>> GetAllCategoriesAsync();

        Task<List<BlogTag>> GetAllTagsAsync();

        Task<string> GetAboutHtmlAsync();

        Task CloneToLocalStoreAsync();

        Task UpdateRemoteStoreTemplatePostAsync();

        Task UpdateMemoryStoreAsync();

        Task UpdateLocalStoreAsync();

        Task UpdateRemoteStoreAsync();
    }
}
