using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog
{
    public interface IBlogAssetManager
    {
        List<BlogPost> AllPosts { get; }

        List<BlogCategory> AllCategories { get; }

        List<BlogTag> AllTags { get; }

        string AboutHtml { get; }

        Task CloneToLocalStoreAsync();

        Task UpdateRemoteStoreTemplatePostAsync();

        Task ReloadLocalMemoryPostAsync();

        Task ReloadLocalMemoryCategoryAsync();

        Task ReloadLocalMemoryTagAsync();

        Task ReloadLocalMemoryAboutAsync();

        Task UpdateLocalStoreAsync();

        Task UpdateRemoteStoreAsync();
    }
}
