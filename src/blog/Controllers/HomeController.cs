using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Blog.HttpService;
using Laobian.Blog.Models;
using Laobian.Share.Extension;
using Laobian.Share.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogConfig _blogConfig;
        private readonly ApiHttpService _apiHttpService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<BlogConfig> config, ILogger<HomeController> logger, ApiHttpService apiHttpService)
        {
            _logger = logger;
            _blogConfig = config.Value;
            _apiHttpService = apiHttpService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int p)
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var posts = await _apiHttpService.GetPostsAsync(!authenticated);
            var model = new PagedPostViewModel(p, posts.Count, _blogConfig.PostsPerPage) { Url = Request.Path };

            foreach (var blogPost in posts.ToPaged(_blogConfig.PostsPerPage, model.CurrentPage))
            {
                var postViewModel = new PostViewModel {Current = blogPost};
                postViewModel.SetAdditionalInfo();
                model.Posts.Add(postViewModel);
            }

            return View(model);
        }

        [HttpGet]
        [Route("/archive")]
        public async Task<IActionResult> Archive()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var posts = await _apiHttpService.GetPostsAsync(!authenticated);
            var model = new List<PostArchiveViewModel>();
            foreach (var item in posts.GroupBy(x => x.Metadata.PublishTime.Year).OrderByDescending(y => y))
            {
                var archiveViewModel = new PostArchiveViewModel();
                archiveViewModel.Count = item.Count();
                archiveViewModel.Posts = item.OrderByDescending(x => x.Metadata.PublishTime).ToList();
                archiveViewModel.Link = $"{item.Key}";
                archiveViewModel.Name = $"{item.Key}年";
                archiveViewModel.BaseUrl = "/archive";
                model.Add(archiveViewModel);
            }

            return View("~/Views/Archive/Index.cshtml", model);
        }

        [HttpGet]
        [Route("/tag")]
        public async Task<IActionResult> Tag()
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var tags = await _apiHttpService.GetTagsAsync();
            var posts = await _apiHttpService.GetPostsAsync(!authenticated);
            var model = new List<PostArchiveViewModel>();

            foreach (var blogTag in tags.OrderByDescending(x => x.LastUpdatedAt))
            {
                var tagPosts = posts.Where(x => x.Metadata.Tags.Contains(blogTag.Link)).ToList();
                var archiveViewModel = new PostArchiveViewModel();
                archiveViewModel.Count = tagPosts.Count();
                archiveViewModel.Posts = tagPosts.OrderByDescending(x => x.Metadata.PublishTime).ToList();
                archiveViewModel.Link = $"{blogTag.Link}";
                archiveViewModel.Name = $"{blogTag.DisplayName}";
                archiveViewModel.BaseUrl = "/tag";
                model.Add(archiveViewModel);
            }

            return View("~/Views/Archive/Index.cshtml", model);
        }

        [HttpGet]
        [Route("/{year:int}/{month:int}/{link}.html")]
        public async Task<IActionResult> Post([FromRoute]int year, [FromRoute] int month, [FromRoute] string link)
        {
            var authenticated = User.Identity?.IsAuthenticated ?? false;
            var posts = await _apiHttpService.GetPostsAsync(!authenticated);
            var post = posts.FirstOrDefault(x => StringHelper.EqualIgnoreCase(x.Link, link));
            if (post == null)
            {
                return NotFound();
            }

            if (post.Metadata.PublishTime.Year != year || post.Metadata.PublishTime.Month != month)
            {
                return NotFound();
            }

            var previousPost = posts.OrderByDescending(x => x.Metadata.PublishTime)
                .FirstOrDefault(x => x.Metadata.PublishTime < post.Metadata.PublishTime);
            var nextPost = posts.OrderBy(x => x.Metadata.PublishTime)
                .FirstOrDefault(x => x.Metadata.PublishTime > post.Metadata.PublishTime);
            var model = new PostViewModel
            {
                Current = post,
                Previous = previousPost,
                Next = nextPost
            };
            model.SetAdditionalInfo();
            return View("~/Views/Post/Index.cshtml", model);
        }

        [HttpGet]
        [Route("/about")]
        public async Task<IActionResult> About()
        {
            //var posts = _blogService.GetPosts(onlyPublic: true, publishTimeDesc: true, toppingPostsFirst: false);
            //var tags = _blogService.GetTags(onlyPublic: true, publishTimeDesc: true, toppingPostsFirst: false);
            //var categories = _blogService.GetCategories(onlyPublic: true, publishTimeDesc: true, toppingPostsFirst: false);
            //var model = new AboutViewModel
            //{
            //    LatestPost = posts.FirstOrDefault(),
            //    PostTotalAccessCount = posts.Sum(p => p.AccessCount).ToString(),
            //    PostTotalCount = posts.Count.ToString(),
            //    TopPosts = posts.OrderByDescending(p => p.AccessCount).Take(Global.Config.Blog.PostsPerPage),
            //    SystemAppVersion = Global.AppVersion,
            //    SystemDotNetVersion = Global.RuntimeVersion,
            //    SystemLastBoot = Global.StartTime.ToChinaDateAndTime(),
            //    SystemRunningInterval = Global.RunningInterval,
            //    TagTotalCount = tags.Count.ToString(),
            //    TopTags = tags.OrderByDescending(t => t.Posts.Count).Take(Global.Config.Blog.PostsPerPage),
            //    CategoryTotalCount = categories.Count.ToString(),
            //    TopCategories = categories.OrderByDescending(c => c.Posts.Count).Take(Global.Config.Blog.PostsPerPage)
            //};

            return View("~/Views/About/Index.csthml", new AboutViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}