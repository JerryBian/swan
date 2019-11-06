using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog.Asset
{
    public interface IBlogAssetManager
    {
        Task<List<BlogPost>> GetAllPostsAsync();

        Task<List<BlogCategory>> GetAllCategoriesAsync();

        Task<List<BlogTag>> GetAllTagsAsync();

        Task<string> GetAboutHtmlAsync();

        Task ReloadLocalFileStoreAsync();

        Task UpdateRemoteStoreTemplatePostAsync();

        Task UpdateMemoryStoreAsync();

        Task UpdateLocalStoreAsync();

        Task UpdateRemoteStoreAsync();
    }
}
