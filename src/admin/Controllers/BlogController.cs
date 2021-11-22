using System;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Admin.Models;
using Laobian.Share;
using Laobian.Share.Site.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("blog")]
public class BlogController : Controller
{
    private readonly AdminOptions _options;
    private readonly ApiSiteHttpClient _apiSiteHttpClient;
    private readonly BlogSiteHttpClient _blogSiteHttpClient;

    public BlogController(ApiSiteHttpClient apiSiteHttpClient, BlogSiteHttpClient blogSiteHttpClient, IOptions<AdminOptions> options)
    {
        _options = options.Value;
        _apiSiteHttpClient = apiSiteHttpClient;
        _blogSiteHttpClient = blogSiteHttpClient;
    }

    [HttpPost]
    [Route("cache/reload")]
    public async Task<IActionResult> ReloadAsync()
    {
        await _blogSiteHttpClient.ReloadBlogDataAsync();
        return Ok();
    }

    [Route("posts")]
    public async Task<IActionResult> GetPostsAsync()
    {
        var posts = await _apiSiteHttpClient.GetPostsAsync(false);
        var tags = await _apiSiteHttpClient.GetTagsAsync();
        var viewModel = posts.OrderByDescending(x => x.Raw.LastUpdateTime).ToList();
        return View("Posts", viewModel);
    }

    [HttpGet("posts/add")]
    public async Task<IActionResult> AddPost()
    {
        var tags = await _apiSiteHttpClient.GetTagsAsync();
        return View("AddPost", tags);
    }

    [HttpPost("posts/add")]
    public async Task<IActionResult> AddPost([FromForm] BlogPost post)
    {
        post.ContainsMath = Request.Form["containsMath"] == "on";
        post.IsPublished = Request.Form["isPublished"] == "on";
        post.IsTopping = Request.Form["isTopping"] == "on";
        var result = await _apiSiteHttpClient.AddPostAsync(post);
        return Redirect(result.Raw.GetFullPath(_options.BlogRemoteEndpoint));
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

    [HttpPost("posts/{postLink}")]
    public async Task<IActionResult> UpdatePost([FromForm] BlogPost post, [FromRoute] string postLink)
    {
        post.ContainsMath = Request.Form["containsMath"] == "on";
        post.IsPublished = Request.Form["isPublished"] == "on";
        post.IsTopping = Request.Form["isTopping"] == "on";
        var result = await _apiSiteHttpClient.UpdatePostAsync(post, postLink);
        return Redirect(result.Raw.GetFullPath(_options.BlogRemoteEndpoint));
    }

    //[Route("posts/{link}")]
    //public async Task<BlogPostRuntime> GetPostAsync([FromRoute] string link)
    //{
    //    var response = new ApiResponse<BlogPostRuntime>();
    //    var post = await _apiSiteHttpClient.GetPostAsync(link);
    //    return post;
    //}

    [Route("tags")]
    public async Task<IActionResult> GetTagsAsync()
    {
        var tags = await _apiSiteHttpClient.GetTagsAsync();
        return View("Tags", tags.OrderByDescending(x => x.LastUpdatedAt));
    }

    //[Route("tags/{id}")]
    //public async Task<BlogTag> GetTagAsync([FromRoute] string id)
    //{
    //    var tag = await _apiSiteHttpClient.GetTagAsync(id);
    //    return tag;
    //}

    [HttpDelete]
    [Route("tags/{id}")]
    public async Task<IActionResult> DeleteTagAsync([FromRoute] string id)
    {
        await _apiSiteHttpClient.DeleteTagAsync(id);
        return Redirect("/blog/tags");
    }

    [HttpGet("tags/add")]
    public IActionResult AddTag()
    {
        return View("AddTag");
    }

    [HttpPost]
    [Route("tags")]
    public async Task<IActionResult> AddTagAsync([FromForm] BlogTag tag)
    {
        await _apiSiteHttpClient.AddTagAsync(tag);
        return Redirect("/blog/tags");
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

    [HttpPost]
    [Route("tags/{id}")]
    public async Task<IActionResult> UpdateTagAsync([FromForm] BlogTag tag, [FromRoute] string id)
    {
        await _apiSiteHttpClient.UpdateTagAsync(tag);
        return Redirect("/blog/tags");
    }
}