using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.ViewComponents
{
    public class PostRecommendViewComponent : ViewComponent
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public PostRecommendViewComponent(IBlogService blogService, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string[] excludedLink)
        {
            var posts = _blogService.GetPosts()
                .Where(p => p.IsPublic && (excludedLink == null || !excludedLink.Contains(p.Link)))
                .ToList();
            return View(posts);
        }
    }
}
