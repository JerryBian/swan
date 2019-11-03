using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Config;
using Laobian.Share.Git;
using Laobian.Share.Infrastructure.Email;

namespace Laobian.Share.Blog
{
    public class BlogAssetManager : IBlogAssetManager
    {
        private readonly List<BlogPost> _allPosts;
        private readonly List<BlogCategory> _allCategories;
        private readonly List<BlogTag> _allTags;
        private readonly AppConfig _appConfig;
        private readonly IGitClient _gitClient;
        private readonly IEmailClient _emailClient;

        public BlogAssetManager()
        {
            _allTags = new List<BlogTag>();
            _allPosts = new List<BlogPost>();
            _allCategories = new List<BlogCategory>();
        }

        public List<BlogPost> AllPosts => _allPosts;

        public List<BlogCategory> AllCategories => _allCategories;

        public List<BlogTag> AllTags => _allTags;

        public string AboutHtml { get; }

        public Task CloneToLocalStoreAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateRemoteStoreTemplatePostAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReloadLocalMemoryPostAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReloadLocalMemoryCategoryAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReloadLocalMemoryTagAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReloadLocalMemoryAboutAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateLocalStoreAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateRemoteStoreAsync()
        {
            throw new NotImplementedException();
        }
    }
}
