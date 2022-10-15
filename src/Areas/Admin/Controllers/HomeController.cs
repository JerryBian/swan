using Laobian.Areas.Admin.Models;
using Laobian.Lib;
using Laobian.Lib.Model;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Areas.Admin.Controllers
{
    [Area(Constants.AreaAdmin)]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogService _blogService;
        private readonly IReadService _readService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public HomeController(
            ILogger<HomeController> logger,
            IReadService readService,
            IBlogService blogService,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _readService = readService;
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

            List<ReadItemView> allReadItems = await _readService.GetAllAsync();
            model.ReadItemTotal = allReadItems.Count;
            model.ReadItemPublic = allReadItems.Count(x => x.Raw.IsPublic);
            model.ReadItemPrivate = allReadItems.Count(x => !x.Raw.IsPublic);

            ViewData["Title"] = "Admin";
            return View(model);
        }

    }
}
