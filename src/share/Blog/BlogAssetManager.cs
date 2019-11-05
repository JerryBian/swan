using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Blog.Parser;
using Laobian.Share.Config;
using Laobian.Share.Git;
using Laobian.Share.Infrastructure.Email;
using Laobian.Share.Log;
using Markdig;
using Microsoft.Extensions.Options;

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
        private readonly BlogPostParser _postParser;
        private readonly BlogCategoryParser _categoryParser;
        private readonly BlogTagParser _tagParser;
        private readonly ManualResetEventSlim _manualReset;
        private readonly SemaphoreSlim _semaphore;
        private readonly ILogService _logService;

        private string _aboutHtml;

        public BlogAssetManager(
            IOptions<AppConfig> appConfig,
            BlogPostParser postParser,
            BlogCategoryParser categoryParser,
            BlogTagParser tagParser,
            IGitClient gitClient,
            IEmailClient emailClient)
        {
            _allTags = new List<BlogTag>();
            _allPosts = new List<BlogPost>();
            _allCategories = new List<BlogCategory>();
            _appConfig = appConfig.Value;
            _gitClient = gitClient;
            _emailClient = emailClient;
            _postParser = postParser;
            _categoryParser = categoryParser;
            _tagParser = tagParser;
            _semaphore = new SemaphoreSlim(1, 1);
            _manualReset = new ManualResetEventSlim(true);
        }

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            await _semaphore.WaitAsync();
            return _allPosts;
        }

        public async Task<List<BlogCategory>> GetAllCategoriesAsync()
        {
            await _semaphore.WaitAsync();
            return _allCategories;
        }

        public async Task<List<BlogTag>> GetAllTagsAsync()
        {
            await _semaphore.WaitAsync();
            return _allTags;
        }

        public async Task<string> GetAboutHtmlAsync()
        {
            await _semaphore.WaitAsync();
            return _aboutHtml;
        }

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
            await _semaphore.WaitAsync();

            var postReloadResult = await ReloadLocalMemoryPostAsync();
            var categoryReloadResult = await ReloadLocalMemoryCategoryAsync();
            var tagReloadResult = await ReloadLocalMemoryTagAsync();
            var aboutReloadResult = await ReloadLocalMemoryAboutAsync();

            var warning = string.Join(Environment.NewLine, postReloadResult.Warning, categoryReloadResult.Warning,
                tagReloadResult.Warning, aboutReloadResult.Warning);
            var error = string.Join(Environment.NewLine, postReloadResult.Error, categoryReloadResult.Error,
                tagReloadResult.Error, aboutReloadResult.Error);
            var subject = "Reload assets successfully";

            if (postReloadResult.Success && categoryReloadResult.Success && tagReloadResult.Success &&
                aboutReloadResult.Success)
            {
                _manualReset.Reset();
                _allPosts.Clear();
                _allPosts.AddRange(postReloadResult.Result);

                _allCategories.Clear();
                _allCategories.AddRange(categoryReloadResult.Result);

                _allTags.Clear();
                _allTags.AddRange(tagReloadResult.Result);

                _aboutHtml = aboutReloadResult.Result;

                _manualReset.Set();
            }
            else
            {
                subject = "Reload assets failed";
            }

            await AlertAsync(subject, warning, error);
            _semaphore.Release();
        }

        public Task UpdateLocalStoreAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateRemoteStoreAsync()
        {
            throw new NotImplementedException();
        }

        private async Task<BlogAssetReloadResult<List<BlogPost>>> ReloadLocalMemoryPostAsync()
        {
            var result = new BlogAssetReloadResult<List<BlogPost>> { Result = new List<BlogPost>() };
            var postLocalPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.PostGitPath);
            if (!Directory.Exists(postLocalPath))
            {
                result.Warning = $"No post folder found under \"{postLocalPath}\".";
                return result;
            }

            foreach (var file in Directory.EnumerateFiles(postLocalPath, $"*{_appConfig.Common.MarkdownExtension}"))
            {
                try
                {
                    var text = await File.ReadAllTextAsync(file);
                    var parseResult = await _postParser.FromTextAsync(text);
                    if (parseResult.WarningMessages.Any())
                    {
                        result.Warning +=
                            $"Parse post warning: {file}.{Environment.NewLine}{string.Join(Environment.NewLine, parseResult.WarningMessages)}{Environment.NewLine}";
                    }

                    if (parseResult.WarningMessages.Any())
                    {
                        result.Warning +=
                            $"Parse post error: {file}.{Environment.NewLine}{string.Join(Environment.NewLine, parseResult.ErrorMessages)}{Environment.NewLine}";
                    }

                    if (parseResult.Success)
                    {
                        result.Result.Add(parseResult.Instance);
                    }
                }
                catch (Exception ex)
                {
                    result.Error +=
                        $"Parse post throw exception: {file}.{Environment.NewLine}{ex}{Environment.NewLine}";
                }
            }

            return result;
        }

        private async Task<BlogAssetReloadResult<List<BlogCategory>>> ReloadLocalMemoryCategoryAsync()
        {
            var result = new BlogAssetReloadResult<List<BlogCategory>>();
            var categoryLocalPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.CategoryGitPath);
            if (!File.Exists(categoryLocalPath))
            {
                result.Warning = $"No category asset found under \"{categoryLocalPath}\".";
                return result;
            }

            var text = await File.ReadAllTextAsync(categoryLocalPath);
            var parseResult = await _categoryParser.FromTextAsync(text);
            if (parseResult.WarningMessages.Any())
            {
                result.Warning =
                    $"Blog category parse warnings: {Environment.NewLine}{string.Join(Environment.NewLine, parseResult.WarningMessages)}";
            }

            if (parseResult.ErrorMessages.Any())
            {
                result.Warning =
                    $"Blog category parse errors: {Environment.NewLine}{string.Join(Environment.NewLine, parseResult.ErrorMessages)}";
            }

            result.Success = parseResult.Success;
            result.Result = parseResult.Instance;
            return result;
        }

        private async Task<BlogAssetReloadResult<List<BlogTag>>> ReloadLocalMemoryTagAsync()
        {
            var result = new BlogAssetReloadResult<List<BlogTag>>();
            var tagLocalPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.TagGitPath);
            if (!File.Exists(tagLocalPath))
            {
                result.Warning = $"No tag asset found under \"{tagLocalPath}\".";
                return result;
            }

            var text = await File.ReadAllTextAsync(tagLocalPath);
            var parseResult = await _tagParser.FromTextAsync(text);
            if (parseResult.WarningMessages.Any())
            {
                result.Warning =
                    $"Blog tag parse warnings: {Environment.NewLine}{string.Join(Environment.NewLine, parseResult.WarningMessages)}";
            }

            if (parseResult.ErrorMessages.Any())
            {
                result.Warning =
                    $"Blog tag parse errors: {Environment.NewLine}{string.Join(Environment.NewLine, parseResult.ErrorMessages)}";
            }

            result.Success = parseResult.Success;
            result.Result = parseResult.Instance;
            return result;
        }

        private async Task<BlogAssetReloadResult<string>> ReloadLocalMemoryAboutAsync()
        {
            var result = new BlogAssetReloadResult<string>();
            var aboutLocalPath = Path.Combine(_appConfig.Blog.AssetRepoLocalDir, _appConfig.Blog.AboutGitPath);
            if (!File.Exists(aboutLocalPath))
            {
                result.Warning = $"No about asset found under \"{aboutLocalPath}\".";
                _aboutHtml = "Not Exists.";
            }

            var md = await File.ReadAllTextAsync(aboutLocalPath);
            result.Result = Markdown.ToHtml(md);
            return result;
        }

        private async Task AlertAsync(string subject, string warning, string error)
        {
            try
            {
                // send alert email out. 
                var emailEntry = new EmailEntry(_appConfig.Common.AdminEnglishName, _appConfig.Common.AdminEmail)
                {
                    FromName = _appConfig.Common.ReportSenderName,
                    FromAddress = _appConfig.Common.ReportSenderEmail,
                    Subject = subject,
                    HtmlContent = $"<p>Reload assets finished, please check.</p>"
                };

                if (!string.IsNullOrEmpty(warning))
                {
                    emailEntry.HtmlContent += $"<p><strong>Warnings: </strong></p><p><pre><code>{warning}</code></pre></p>";
                }

                if (!string.IsNullOrEmpty(error))
                {
                    emailEntry.HtmlContent += $"<p><strong>Errors: </strong></p><p><pre><code>{error}</code></pre></p>";
                }

                await _emailClient.SendAsync(emailEntry);
            }
            catch(Exception ex)
            {
                await _logService.LogError(
                    $"Alert assets reloading failed, subject = {subject}, warning = {warning}, error = {error}.", ex,
                    true);
            }
        }
    }
}
