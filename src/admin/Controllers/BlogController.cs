using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.Models;
using Laobian.Share.Extension;
using Laobian.Share.Grpc;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Service;
using Laobian.Share.Model.Blog;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("blog")]
public class BlogController : Controller
{
    private readonly BlogGrpcRequest _blogGrpcRequest;
    private readonly IBlogGrpcService _blogGrpcService;
    private readonly ILogger<BlogController> _logger;
    private readonly AdminOptions _options;

    public BlogController(
        IOptions<AdminOptions> options, ILogger<BlogController> logger)
    {
        _logger = logger;
        _options = options.Value;
        _blogGrpcRequest = new BlogGrpcRequest();
        _blogGrpcService = GrpcClientHelper.CreateClient<IBlogGrpcService>(options.Value.ApiLocalEndpoint);
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
            await _blogGrpcService.ReloadBlogCacheAsync(_blogGrpcRequest);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Reload blog cache failed.");
        }

        return response;
    }

    [HttpPost("post/stat/words-count")]
    public async Task<ApiResponse<ChartResponse>> GetPostsWordsCountStats()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            _blogGrpcRequest.ExtractRuntime = true;
            var blogResponse = await _blogGrpcService.GetPostsAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                blogResponse.Posts ??= new List<BlogPostRuntime>();
                var items = blogResponse.Posts.GroupBy(x => x.Raw.CreateTime.Year).OrderBy(x => x.Key);
                var chartResponse = new ChartResponse
                {
                    Title = "当年文章的总字数",
                    Type = "bar"
                };
                foreach (var item in items)
                {
                    chartResponse.Data.Add(item.Sum(x => x.Raw.MdContent.Length));
                    chartResponse.Labels.Add(item.Key.ToString());
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get posts stats for words count failed.");
        }

        return response;
    }

    [HttpPost("post/stat/count")]
    public async Task<ApiResponse<ChartResponse>> GetPostsCountStats()
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            _blogGrpcRequest.ExtractRuntime = true;
            var blogResponse = await _blogGrpcService.GetPostsAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                blogResponse.Posts ??= new List<BlogPostRuntime>();
                var items = blogResponse.Posts.GroupBy(x => x.Raw.CreateTime.Year).OrderBy(x => x.Key);
                var chartResponse = new ChartResponse
                {
                    Title = "当年发表文章数",
                    Type = "bar"
                };
                foreach (var item in items)
                {
                    chartResponse.Data.Add(item.Count());
                    chartResponse.Labels.Add(item.Key.ToString());
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, "Get posts stats for post count failed.");
        }

        return response;
    }

    [HttpPost("post/{postLink}/access")]
    public async Task<ApiResponse<ChartResponse>> GetPostAccessChart([FromRoute] string postLink)
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            _blogGrpcRequest.ExtractRuntime = true;
            _blogGrpcRequest.Link = postLink;
            var blogResponse = await _blogGrpcService.GetPostAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                var chartResponse = new ChartResponse
                {
                    Title = "当天的访问量",
                    Type = "line"
                };

                for (var i = 14; i >= 0; i--)
                {
                    var date = DateTime.Now.Date.AddDays(-i);
                    blogResponse.PostRuntime.Accesses ??= new List<BlogAccess>();
                    var item = blogResponse.PostRuntime.Accesses.FirstOrDefault(x => x.Date == date);
                    chartResponse.Data.Add(item?.Count ?? 0);
                    chartResponse.Labels.Add(date.ToRelativeDaysHuman());
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get post {postLink} access stats failed.");
        }

        return response;
    }

    [HttpPost("post/access")]
    public async Task<ApiResponse<ChartResponse>> GetPostsAccess([FromQuery] int days)
    {
        var response = new ApiResponse<ChartResponse>();
        try
        {
            _blogGrpcRequest.ExtractRuntime = true;
            var blogResponse = await _blogGrpcService.GetPostsAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                blogResponse.Posts ??= new List<BlogPostRuntime>();
                var access = blogResponse.Posts.SelectMany(x => x.Accesses)
                    .Where(x => x.Date >= DateTime.Now.AddDays(-days) && x.Date <= DateTime.Now).GroupBy(x => x.Date)
                    .OrderBy(x => x.Key);
                var chartResponse = new ChartResponse {Title = "访问量", Type = "line"};
                foreach (var item in access)
                {
                    chartResponse.Data.Add(item.Sum(x => x.Count));
                    chartResponse.Labels.Add(item.Key.ToRelativeDaysHuman());
                }

                response.Content = chartResponse;
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Get posts access failed for {days} days.");
        }

        return response;
    }

    [Route("post")]
    public async Task<IActionResult> GetPostsAsync()
    {
        _blogGrpcRequest.ExtractRuntime = true;
        var blogResponse = await _blogGrpcService.GetPostsAsync(_blogGrpcRequest);
        if (blogResponse.IsOk)
        {
            blogResponse.Posts ??= new List<BlogPostRuntime>();
            var viewModel = blogResponse.Posts.OrderByDescending(x => x.Raw.LastUpdateTime).ToList();
            return View("Posts", viewModel);
        }

        return NotFound(blogResponse.Message);
    }

    [HttpGet("post/add")]
    public async Task<IActionResult> AddPost()
    {
        var blogResponse = await _blogGrpcService.GetTagsAsync();
        if (blogResponse.IsOk)
        {
            return View("AddPost", blogResponse.Tags);
        }

        return NotFound(blogResponse.Message);
    }

    [HttpPost("post")]
    public async Task<ApiResponse<object>> AddPost([FromForm] BlogPost post)
    {
        var response = new ApiResponse<object>();
        try
        {
            post.ContainsMath = Request.Form["containsMath"] == "on";
            post.IsPublished = Request.Form["isPublished"] == "on";
            post.IsTopping = Request.Form["isTopping"] == "on";
            _blogGrpcRequest.Post = post;
            var blogResponse = await _blogGrpcService.AddPostAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                response.RedirectTo = blogResponse.Post.GetFullPath(_options.BlogRemoteEndpoint);
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Add post failed. {Environment.NewLine}{JsonUtil.Serialize(post)}");
        }

        return response;
    }

    [HttpGet("post/{postLink}/update")]
    public async Task<IActionResult> UpdatePost([FromRoute] string postLink)
    {
        _blogGrpcRequest.Link = postLink;
        var blogResponse = await _blogGrpcService.GetPostAsync(_blogGrpcRequest);
        if (blogResponse.IsOk)
        {
            var model = new BlogPostUpdateViewModel {Post = blogResponse.PostRuntime.Raw};
            blogResponse = await _blogGrpcService.GetTagsAsync();
            if (blogResponse.IsOk)
            {
                model.Tags.AddRange(blogResponse.Tags);
                return View("UpdatePost", model);
            }

            return NotFound(blogResponse.Message);
        }

        return NotFound(blogResponse.Message);
    }

    [HttpPut("post/{postLink}")]
    public async Task<ApiResponse<object>> UpdatePost([FromForm] BlogPost post, [FromRoute] string postLink)
    {
        var response = new ApiResponse<object>();
        try
        {
            post.ContainsMath = Request.Form["containsMath"] == "on";
            post.IsPublished = Request.Form["isPublished"] == "on";
            post.IsTopping = Request.Form["isTopping"] == "on";
            _blogGrpcRequest.Post = post;
            _blogGrpcRequest.OriginalPostLink = postLink;
            var blogResponse = await _blogGrpcService.UpdatePostAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                response.RedirectTo = blogResponse.Post.GetFullPath(_options.BlogRemoteEndpoint);
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Update post({postLink}) failed: {JsonUtil.Serialize(post)}");
        }

        return response;
    }

    [Route("tag")]
    public async Task<IActionResult> GetTagsAsync()
    {
        var blogResponse = await _blogGrpcService.GetTagsAsync();
        if (blogResponse.IsOk)
        {
            blogResponse.Tags ??= new List<BlogTag>();
            return View("Tags", blogResponse.Tags.OrderByDescending(x => x.LastUpdatedAt));
        }

        return NotFound(blogResponse.Message);
    }

    [HttpDelete]
    [Route("tag/{id}")]
    public async Task<ApiResponse<object>> DeleteTagAsync([FromRoute] string id)
    {
        var response = new ApiResponse<object>();
        try
        {
            _blogGrpcRequest.TagId = id;
            await _blogGrpcService.DeleteTagAsync(_blogGrpcRequest);
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
            _logger.LogError(ex, $"Delete blog tag {id} failed.");
        }

        return response;
    }

    [HttpGet("tag/add")]
    public IActionResult AddTag()
    {
        return View("AddTag");
    }

    [HttpPost]
    [Route("tag")]
    public async Task<ApiResponse<object>> AddTagAsync([FromBody] BlogTag tag)
    {
        var response = new ApiResponse<object>();
        try
        {
            _blogGrpcRequest.Tag = tag;
            var blogResponse = await _blogGrpcService.AddTagAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                response.RedirectTo = "/blog/tag";
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
        }
        catch (Exception ex)
        {
            response.IsOk = false;
            response.Message = ex.Message;
        }

        return response;
    }

    [HttpGet("tag/{id}/update")]
    public async Task<IActionResult> UpdateTagAsync([FromRoute] string id)
    {
        _blogGrpcRequest.TagId = id;
        var blogResponse = await _blogGrpcService.GetTagAsync(_blogGrpcRequest);
        if (blogResponse.IsOk)
        {
            return View("UpdateTag", blogResponse.Tag);
        }

        return NotFound(blogResponse.Message);
    }

    [HttpPut]
    [Route("tag/{id}")]
    public async Task<ApiResponse<object>> UpdateTagAsync([FromForm] BlogTag tag, [FromRoute] string id)
    {
        var response = new ApiResponse<object>();
        try
        {
            _blogGrpcRequest.Tag = tag;
            var blogResponse = await _blogGrpcService.UpdateTagAsync(_blogGrpcRequest);
            if (blogResponse.IsOk)
            {
                response.RedirectTo = "/blog/tag";
            }
            else
            {
                response.IsOk = false;
                response.Message = blogResponse.Message;
            }
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