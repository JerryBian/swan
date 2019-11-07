using System;
using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share.Blog;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public ArchiveController(IOptions<AppConfig> appConfig, IBlogService blogService)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
        }

        [Route("/category")]
        public IActionResult Category()
        {
            var model = User.Identity.IsAuthenticated ? _blogService.GetCategories(false) : _blogService.GetCategories();
            ViewData["Title"] = "分类";
            ViewData["Canonical"] = "/category/";
            ViewData["Description"] = "所有文章以分类的形式展现";
            return View("Category", model);
        }

        [Route("/tag")]
        [ResponseCache(CacheProfileName = "Cache10Sec")]
        public IActionResult Tag()
        {
            var model = User.Identity.IsAuthenticated ? _blogService.GetTags(false) : _blogService.GetTags();
            ViewData["Title"] = "标签";
            ViewData["Canonical"] = "/tag/";
            ViewData["Description"] = "所有文章以标签归类的形式展现";
            return View("Tag", model);
        }

        [Route("/archive")]
        [ResponseCache(CacheProfileName = "Cache10Sec")]
        public IActionResult Date()
        {
            var model = User.Identity.IsAuthenticated ? _blogService.GetArchives(false) : _blogService.GetArchives();
            ViewData["Title"] = "存档";
            ViewData["Canonical"] = "/archive/";
            ViewData["Description"] = "所有文章以发表日期归类的形式展现";
            return View("Index", model);
        }
    }
}