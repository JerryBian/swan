using System;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Admin.Models;
using Laobian.Share;
using Laobian.Share.Extension;
using Laobian.Share.Site.Blog;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("blog")]
public class BlogController : Controller
{
    private readonly ApiSiteHttpClient _apiSiteHttpClient;
    private readonly BlogSiteHttpClient _blogSiteHttpClient;
    private readonly ILogger<BlogController> _logger;
    private readonly AdminOptions _options;

    public BlogController(ApiSiteHttpClient apiSiteHttpClient, BlogSiteHttpClient blogSiteHttpClient,
        IOptions<AdminOptions> options, ILogger<BlogController> logger)
    {
        _logger = logger;
        _options = options.Value;
        _apiSiteHttpClient = apiSiteHttpClient;
        _blogSiteHttpClient = blogSiteHttpClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("cache/reload")]
    public async Task<ApiResponse<object>> ReloadAsync()
    {
        var response = new ApiResponse<object>();
        try
        {
            await _blogSiteHttpClient.ReloadBlogDataAsync();
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Reload blog cache failed.");
        }

        return response;
    }

    [HttpPost("posts/stats/words-count")]
    public async Task<ApiResponse<ChartResponse>> GetPostsWordsCountStats()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var posts = await _apiSiteHttpClient.GetPostsAsync(true);
            var items = posts.GroupBy(x => x.Raw.CreateTime.Year).OrderBy(x => x.Key);
            var chartResponse = new ChartResponse
            {
                Title = "当年发表文章的总字数",
                Type = "line"
            };
            foreach (var item in items)
            {
                chartResponse.Data.Add(item.Sum(x => x.Raw.WordsCount));
                chartResponse.Labels.Add(item.Key.ToString());
            }

            response.Content = chartResponse;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get posts stats for words count failed.");
        }

        return response;
    }

    [HttpPost("posts/stats/count")]
    public async Task<ApiResponse<ChartResponse>> GetPostsCountStats()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var posts = await _apiSiteHttpClient.GetPostsAsync(true);
            var items = posts.GroupBy(x => x.Raw.CreateTime.Year).OrderBy(x => x.Key);
            var chartResponse = new ChartResponse
            {
                Title = "当年发表文章数",
                Type = "line"
            };
            foreach (var item in items)
            {
                chartResponse.Data.Add(item.Count());
                chartResponse.Labels.Add(item.Key.ToString());
            }

            response.Content = chartResponse;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get posts stats for post count failed.");
        }

        return response;
    }

    [HttpPost("posts/access")]
    public async Task<ApiResponse<ChartResponse>> GetPostsAccess([FromQuery] int days)
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            var posts = await _apiSiteHttpClient.GetPostsAsync(true);
            var access = posts.SelectMany(x => x.Accesses)
                .Where(x => x.Date >= DateTime.Now.AddDays(-days) && x.Date <= DateTime.Now).GroupBy(x => x.Date)
                .OrderBy(x => x.Key);
            var chartResponse = new ChartResponse {Title = "访问量", Type = "line"};
            foreach (var item in access)
            {
                chartResponse.Data.Add(item.Count());
                chartResponse.Labels.Add(item.Key.ToRelativeDaysHuman());
            }

            response.Content = chartResponse;
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get posts access failed for {days} days.");
        }

        return response;
    }

    [Route("posts")]
    public async Task<IActionResult> GetPostsAsync()
    {
        var posts = await _apiSiteHttpClient.GetPostsAsync(true);
        var viewModel = posts.OrderByDescending(x => x.Raw.LastUpdateTime).ToList();
        return View("Posts", viewModel);
    }

    [HttpGet("posts/add")]
    public async Task<IActionResult> AddPost()
    {
        var tags = await _apiSiteHttpClient.GetTagsAsync();
        return View("AddPost", tags);
    }

    [HttpPost("posts")]
    public async Task<ApiResponse<object>> AddPost([FromForm] BlogPost post)
    {
        var response = new ApiResponse<object>();
        try
        {
            post.ContainsMath = Request.Form["containsMath"] == "on";
            post.IsPublished = Request.Form["isPublished"] == "on";
            post.IsTopping = Request.Form["isTopping"] == "on";
            var result = await _apiSiteHttpClient.AddPostAsync(post);
            response.RedirectTo = result.GetFullPath(_options.BlogRemoteEndpoint);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Add post failed. {Environment.NewLine}{JsonUtil.Serialize(post)}");
        }

        return response;
    }

    [HttpGet("posts/{postLink}/update")]
    public async Task<IActionResult> UpdatePost([FromRoute] string postLink)
    {
        var post = await _apiSiteHttpClient.GetPostAsync(postLink);
        if (post == null)
        {
            return NotFound($"Blog post with link \"{postLink}\" not found.");
        }

        var model = new BlogPostUpdateViewModel {Post = post.Raw};
        var tags = await _apiSiteHttpClient.GetTagsAsync();
        if (tags != null)
        {
            model.Tags.AddRange(tags);
        }

        return View("UpdatePost", model);
    }

    [HttpPut("posts/{postLink}")]
    public async Task<ApiResponse<object>> UpdatePost([FromForm] BlogPost post, [FromRoute] string postLink)
    {
        var response = new ApiResponse<object>();
        try
        {
            post.ContainsMath = Request.Form["containsMath"] == "on";
            post.IsPublished = Request.Form["isPublished"] == "on";
            post.IsTopping = Request.Form["isTopping"] == "on";
            var result = await _apiSiteHttpClient.UpdatePostAsync(post, postLink);
            response.RedirectTo = result.GetFullPath(_options.BlogRemoteEndpoint);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update post({postLink}) failed: {JsonUtil.Serialize(post)}");
        }

        return response;
    }

    [Route("tags")]
    public async Task<IActionResult> GetTagsAsync()
    {
        var tags = await _apiSiteHttpClient.GetTagsAsync();
        return View("Tags", tags.OrderByDescending(x => x.LastUpdatedAt));
    }

    [HttpDelete]
    [Route("tags/{id}")]
    public async Task<ApiResponse<object>> DeleteTagAsync([FromRoute] string id)
    {
        var response = new ApiResponse<object>();
        try
        {
            await _apiSiteHttpClient.DeleteTagAsync(id);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Delete blog tag {id} failed.");
        }

        return response;
    }

    [HttpGet("tags/add")]
    public IActionResult AddTag()
    {
        return View("AddTag");
    }

    [HttpPost]
    [Route("tags")]
    public async Task<ApiResponse<object>> AddTagAsync([FromBody] BlogTag tag)
    {
        var response = new ApiResponse<object>();
        try
        {
            await _apiSiteHttpClient.AddTagAsync(tag);
            response.RedirectTo = "/blog/tags";
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
        }

        return response;
    }

    [HttpGet("tags/{id}/update")]
    public async Task<IActionResult> UpdateTagAsync([FromRoute] string id)
    {
        var tag = await _apiSiteHttpClient.GetTagAsync(id);
        if (tag != null)
        {
            return View("UpdateTag", tag);
        }

        return NotFound($"Tag with id \"{id}\" not found.");
    }

    [HttpPut]
    [Route("tags/{id}")]
    public async Task<ApiResponse<object>> UpdateTagAsync([FromForm] BlogTag tag, [FromRoute] string id)
    {
        var response = new ApiResponse<object>();
        try
        {
            await _apiSiteHttpClient.UpdateTagAsync(tag);
            response.RedirectTo = "/blog/tags";
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update tag failed. {JsonUtil.Serialize(tag)}");
        }

        return response;
    }
}