using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swan.Areas.Admin.Models;
using Swan.Core;
using Swan.Core.Model;
using Swan.Lib.Model;
using Swan.Lib.Service;

namespace Swan.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogService _blogService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public HomeController(
            ILogger<HomeController> logger,
            IBlogService blogService,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _blogService = blogService;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        [HttpPost("/admin/shutdown")]
        public IActionResult StopApplication()
        {
            ApiResponse<string> res = new();
            try
            {
                _logger.LogInformation($"Request to stop app, IP={Request.HttpContext.Connection.RemoteIpAddress}");
                _hostApplicationLifetime.StopApplication();
                res.Content = "Request app stop successfully.";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = $"Request stop app failed.";
                _logger.LogError(ex, res.Message);
            }

            return Json(res);
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel model = new();
            List<BlogPostView> allPosts = await _blogService.GetAllPostsAsync();
            model.BlogPostTotal = allPosts.Count;
            model.BlogPostPublic = allPosts.Count(x => x.IsPublishedNow);
            model.BlogPostPrivate = allPosts.Count(x => !x.IsPublishedNow);
            model.BlogPostVisitTotal = allPosts.Sum(x => x.Raw.AccessCount);

            ViewData["Title"] = "Admin";
            return View(model);
        }

    }
}
