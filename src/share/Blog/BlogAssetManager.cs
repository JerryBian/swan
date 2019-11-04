using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Config;
using Laobian.Share.Git;
using Laobian.Share.Infrastructure.Email;
using Markdig;

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

        private string _aboutHtml;

        public BlogAssetManager()
        {
            _allTags = new List<BlogTag>();
            _allPosts = new List<BlogPost>();
            _allCategories = new List<BlogCategory>();
        }

        public List<BlogPost> AllPosts => _allPosts;

        public List<BlogCategory> AllCategories => _allCategories;

        public List<BlogTag> AllTags => _allTags;

        public string AboutHtml => _aboutHtml;

        public Task CloneToLocalStoreAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateRemoteStoreTemplatePostAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateMemoryStoreAsync()
        {
            await Task.WhenAll(new[]
            {
                ReloadLocalMemoryAboutAsync(),
                ReloadLocalMemoryCategoryAsync(),
                ReloadLocalMemoryPostAsync(),
                ReloadLocalMemoryTagAsync()
            });
        }

        public Task UpdateLocalStoreAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateRemoteStoreAsync()
        {
            throw new NotImplementedException();
        }

        private async Task ReloadLocalMemoryPostAsync()
        {

        }

        private async Task ReloadLocalMemoryCategoryAsync()
        {

        }

        private async Task ReloadLocalMemoryTagAsync()
        {

        }

        private async Task ReloadLocalMemoryAboutAsync()
        {
            var aboutLocalPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.AboutGitPath);
            if (!File.Exists(aboutLocalPath))
            {
                _aboutHtml = "Not Exists.";
            }

            var md = await File.ReadAllTextAsync(aboutLocalPath);
            _aboutHtml = Markdown.ToHtml(md);
        }
    }
}
