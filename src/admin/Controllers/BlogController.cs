using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Admin.Models;
using Laobian.Share.Blog;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers
{
    [Route("blog")]
    public class BlogController : Controller
    {
        private readonly ApiSiteHttpClient _apiSiteHttpClient;
        private readonly BlogSiteHttpClient _blogSiteHttpClient;

        public BlogController(ApiSiteHttpClient apiSiteHttpClient, BlogSiteHttpClient blogSiteHttpClient)
        {
            _apiSiteHttpClient = apiSiteHttpClient;
            _blogSiteHttpClient = blogSiteHttpClient;
        }

        [HttpPost]
        [Route("reload")]
        public async Task<IActionResult> ReloadAsync()
        {
            await _blogSiteHttpClient.ReloadBlogDataAsync();
            return Ok();
        }

        [HttpPost]
        [Route("persistent")]
        public async Task<bool> PersistentAsync()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var message = await reader.ReadToEndAsync();
            return await _apiSiteHttpClient.PersistentAsync(message);
        }

        [Route("post")]
        public async Task<IActionResult> GetPostsAsync()
        {
            var posts = await _apiSiteHttpClient.GetPostsAsync();
            var tags = await _apiSiteHttpClient.GetTagsAsync();
            var viewModel = posts.OrderByDescending(x => x.Raw.LastUpdateTime).ToList();
            return View("Posts", viewModel);
        }

        [HttpGet("post/add")]
        public async Task<IActionResult> AddPost()
        {
            var tags = await _apiSiteHttpClient.GetTagsAsync();
            return View("AddPost", tags);
        }

        [HttpPost("post/add")]
        public async Task<IActionResult> AddPost([FromForm] BlogPost post)
        {
            post.ContainsMath = Request.Form["containsMath"] == "on";
            post.IsPublished = Request.Form["isPublished"] == "on";
            post.IsTopping = Request.Form["isTopping"] == "on";
            await _apiSiteHttpClient.AddPostAsync(post);
            return Redirect("/blog/post");
        }

        [HttpGet("post/update/{postLink}")]
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

        [HttpPost("post/update")]
        public async Task<IActionResult> UpdatePost([FromForm] BlogPost post)
        {
            post.ContainsMath = Request.Form["containsMath"] == "on";
            post.IsPublished = Request.Form["isPublished"] == "on";
            post.IsTopping = Request.Form["isTopping"] == "on";
            await _apiSiteHttpClient.UpdatePostAsync(post);
            return Redirect("/blog/post");
        }

        [Route("post/{link}")]
        public async Task<BlogPostRuntime> GetPostAsync([FromRoute] string link)
        {
            var post = await _apiSiteHttpClient.GetPostAsync(link);
            return post;
        }


        [Route("tag")]
        public async Task<IActionResult> GetTagsAsync()
        {
            var tags = await _apiSiteHttpClient.GetTagsAsync();
            return View("Tags", tags.OrderByDescending(x => x.LastUpdatedAt));
        }

        [Route("tag/{link}")]
        public async Task<BlogTag> GetTagAsync([FromRoute] string link)
        {
            var tag = await _apiSiteHttpClient.GetTagAsync(link);
            return tag;
        }

        [HttpDelete]
        [Route("tag/{link}")]
        public async Task<bool> DeleteTagAsync([FromRoute] string link)
        {
            var result = await _apiSiteHttpClient.DeleteTagAsync(link);
            return result;
        }

        [HttpGet("tag/add")]
        public IActionResult AddTag()
        {
            return View("AddTag");
        }

        [HttpPost]
        [Route("tag/add")]
        public async Task<IActionResult> AddTagAsync([FromForm] BlogTag tag)
        {
            await _apiSiteHttpClient.AddTagAsync(tag);
            return Redirect("/blog/tag");
        }

        [HttpGet("tag/update/{tagLink}")]
        public async Task<IActionResult> UpdateTagAsync([FromRoute] string tagLink)
        {
            var tag = await _apiSiteHttpClient.GetTagAsync(tagLink);
            if (tag != null)
            {
                return View("UpdateTag", tag);
            }

            return NotFound($"Tag with link \"{tagLink}\" not found.");
        }

        [HttpPost]
        [Route("tag/update")]
        public async Task<IActionResult> UpdateTagAsync([FromForm] BlogTag tag)
        {
            await _apiSiteHttpClient.UpdateTagAsync(tag);
            return Redirect("/blog/tag");
        }
    }
}