using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Blog;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Api.Controllers
{
    [ApiController]
    [Route("blog")]
    public class BlogApiController : ControllerBase
    {
        private readonly LaobianApiOption _laobianApiOption;
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<BlogApiController> _logger;

        public BlogApiController(IOptions<LaobianApiOption> apiOption, IFileRepository fileRepository, ILogger<BlogApiController> logger)
        {
            _logger = logger;
            _laobianApiOption = apiOption.Value;
            _fileRepository = fileRepository;
        }

        [HttpPost]
        [Route("persistent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PersistentAsync()
        {
            try
            {
                using var sr = new StreamReader(Request.Body, Encoding.UTF8);
                var message = await sr.ReadToEndAsync();
                await _fileRepository.SaveAsync(message);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PersistentAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogPostRuntime>>> GetPostsAsync([FromQuery] bool onlyPublished)
        {
            try
            {
                var posts = await _fileRepository.GetBlogPostsAsync();
                var result = new List<BlogPostRuntime>();
                foreach (var blogPost in posts)
                {
                    var blogPostRuntime = await GetBlogPostRuntimeAsync(blogPost);
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
        [Route("post/{link}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlogPostRuntime>> GetPostAsync([FromRoute] string link)
        {
            try
            {
                var post = await _fileRepository.GetBlogPostAsync(link);
                if (post == null)
                {
                    return NotFound($"Post with link not found: {link}");
                }

                var blogPostRuntime = await GetBlogPostRuntimeAsync(post);
                return Ok(blogPostRuntime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetPostAsync)}({link}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<BlogPostRuntime> GetBlogPostRuntimeAsync(BlogPost post)
        {
            var blogPostRuntime = new BlogPostRuntime(post);
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

            blogPostRuntime.ExtractRuntimeData(_laobianApiOption, blogPostAccess, blogTags);
            return blogPostRuntime;
        }

        [HttpPut]
        [Route("post")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddBlogPost(BlogPost post)
        {
            try
            {
                await _fileRepository.AddBlogPostAsync(post);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddBlogPost)}({JsonUtil.Serialize(post)}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("post")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBlogPost(BlogPost post)
        {
            try
            {
                await _fileRepository.UpdateBlogPostAsync(post);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateBlogPost)}({JsonUtil.Serialize(post)}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("post/access/{link}")]
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
        [Route("tag")]
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
        [Route("tag/{link}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogTag>> GetTagAsync([FromRoute] string link)
        {
            try
            {
                var tag = await _fileRepository.GetBlogTagAsync(link);
                if (tag == null)
                {
                    return NotFound($"Tag link = {link} not found.");
                }

                return tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("tag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogTag>> AddTagAsync(BlogTag tag)
        {
            try
            {
                await _fileRepository.AddBlogTagAsync(tag);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("tag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogTag>> UpdateTagAsync(BlogTag tag)
        {
            try
            {
                await _fileRepository.UpdateBlogTagAsync(tag);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("tag/{link}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteTagAsync([FromRoute] string link)
        {
            try
            {
                await _fileRepository.DeleteBlogTagAsync(link);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DeleteTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}