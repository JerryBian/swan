using System;
using System.IO;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Infrastructure.Cache;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly IMemoryCacheClient _cacheClient;

        public AboutController(IMemoryCacheClient cacheClient, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _cacheClient = cacheClient;
        }

        public async Task<IActionResult> Index()
        {
            var html = await _cacheClient.GetOrAddAsync(BlogConstant.EnglishAboutMemCacheKey, async () =>
            {
                var localPath = Path.Combine(_appConfig.AssetRepoLocalDir, BlogConstant.EnAboutGitHub);
                if (!System.IO.File.Exists(localPath))
                {
                    return string.Empty;
                }

                var md = await System.IO.File.ReadAllTextAsync(localPath);
                return Markdown.ToHtml(md);
            }, TimeSpan.FromDays(1));

            ViewData["Title"] = "关于";
            return View(model: html);
        }
    }
}
