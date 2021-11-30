using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Laobian.Api.HttpClients;
using Laobian.Api.Repository;
using Laobian.Share.Site.Blog;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Controllers;

[ApiController]
[Route("blog")]
public class BlogApiController : ControllerBase
{
    private readonly BlogSiteHttpClient _blogSiteHttpClient;
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<BlogApiController> _logger;

    public BlogApiController(IFileRepository fileRepository,
        ILogger<BlogApiController> logger, BlogSiteHttpClient blogSiteHttpClient)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _blogSiteHttpClient = blogSiteHttpClient;
    }

    [HttpGet]
    [Route("posts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<BlogPostRuntime>>> GetPostsAsync([FromQuery] bool extractRuntime = true)
    {
        try
        {
            var posts = await _fileRepository.GetBlogPostsAsync();
            var result = new List<BlogPostRuntime>();
            foreach (var blogPost in posts)
            {
                var blogPostRuntime = await GetBlogPostRuntimeAsync(blogPost, extractRuntime);
                result.Add(blogPostRuntime);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetPostsAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("posts/{link}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlogPostRuntime>> GetPostAsync([FromRoute] string link,
        [FromQuery] bool extractRuntime = true)
    {
        try
        {
            var post = await _fileRepository.GetBlogPostAsync(link);
            if (post == null)
            {
                return NotFound($"Post with link not found: {link}");
            }

            var blogPostRuntime = await GetBlogPostRuntimeAsync(post, extractRuntime);
            return Ok(blogPostRuntime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetPostAsync)}({link}) failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private async Task<BlogPostRuntime> GetBlogPostRuntimeAsync(BlogPost post, bool extractRuntime = true)
    {
        var blogPostRuntime = new BlogPostRuntime(post);
        if (extractRuntime)
        {
            var blogPostAccess = await _fileRepository.GetBlogPostAccessAsync(post.Link);
            var blogTags = new List<BlogTag>();
            foreach (var blogPostTag in post.Tag)
            {
                var tag = await _fileRepository.GetBlogTagAsync(blogPostTag);
                if (tag != null)
                {
                    blogTags.Add(tag);
                }
            }

            blogPostRuntime.ExtractRuntimeData(blogPostAccess, blogTags);
        }

        return blogPostRuntime;
    }

    [HttpPost]
    [Route("posts")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddBlogPost(BlogPost post)
    {
        try
        {
            await _fileRepository.AddBlogPostAsync(post);
            await _blogSiteHttpClient.ReloadBlogDataAsync();
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(AddBlogPost)}({JsonUtil.Serialize(post)}) failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    [Route("posts")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBlogPost(BlogPost post, [FromQuery] string replacedPostLink)
    {
        try
        {
            await _fileRepository.UpdateBlogPostAsync(post, replacedPostLink);
            await _blogSiteHttpClient.ReloadBlogDataAsync();
            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateBlogPost)}({JsonUtil.Serialize(post)}) failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("posts/{link}/access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddBlogPostAccess([FromRoute] string link)
    {
        try
        {
            var post = await _fileRepository.GetBlogPostAsync(link);
            if (post == null)
            {
                _logger.LogWarning($"No post found with link {link}, new access will be discarded.");
            }
            else
            {
                await _fileRepository.AddBlogPostAccessAsync(post, DateTime.Now, 1);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(AddBlogPostAccess)}({link}) failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<BlogTag>>> GetTagsAsync()
    {
        try
        {
            var tags = await _fileRepository.GetBlogTagsAsync();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetTagsAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet]
    [Route("tags/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BlogTag>> GetTagAsync([FromRoute] string id)
    {
        try
        {
            var tag = await _fileRepository.GetBlogTagAsync(id);
            if (tag == null)
            {
                return NotFound($"Tag id = {id} not found.");
            }

            return tag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetTagAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [Route("tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BlogTag>> AddTagAsync(BlogTag tag)
    {
        try
        {
            await _fileRepository.AddBlogTagAsync(tag);
            await _blogSiteHttpClient.ReloadBlogDataAsync();
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(AddTagAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPut]
    [Route("tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BlogTag>> UpdateTagAsync(BlogTag tag)
    {
        try
        {
            await _fileRepository.UpdateBlogTagAsync(tag);
            await _blogSiteHttpClient.ReloadBlogDataAsync();
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateTagAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpDelete]
    [Route("tags/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTagAsync([FromRoute] string id)
    {
        try
        {
            await _fileRepository.DeleteBlogTagAsync(id);
            var posts = await _fileRepository.GetBlogPostsAsync();
            foreach (var blogPost in posts)
            {
                if (blogPost.Tag.Contains(id))
                {
                    blogPost.Tag.Remove(id);
                    await _fileRepository.UpdateBlogPostAsync(blogPost, blogPost.Link);
                }
            }

            await _blogSiteHttpClient.ReloadBlogDataAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteTagAsync)} failed.");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}