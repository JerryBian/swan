using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Infrastructure.GitHub;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("github")]
    public class GitHubController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public GitHubController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // http://michaco.net/blog/HowToValidateGitHubWebhooksInCSharpWithASPNETCoreMVC
        [HttpPost]
        [Route("hook")]
        public async Task<IActionResult> Hook(GitHubPayload payload)
        {
            if (!Request.Headers.ContainsKey("X-GitHub-Event"))
            {
                return BadRequest();
            }
            var gitHubEvent = Request.Headers["X-GitHub-Event"];
            if (!string.Equals("push", gitHubEvent, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            if (!Request.Headers.ContainsKey("X-Hub-Signature"))
            {
                return BadRequest();
            }

            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes("abc"));
                var a = Convert.ToBase64String(hash);
                var d = string.Concat(hash.Select(b => b.ToString("x2")));

            }
            //if(!string.Equals())

            if (payload.Commits.Any(_ => GitHubMessageProvider.IsServerCommit(_.Message)))
            {
                return Ok("Server update, no need to update local.");
            }

            await _blogService.UpdateLocalAssetsAsync();

            var modifiedPosts = payload.Commits.SelectMany(c => c.Modified).ToList();
            if (modifiedPosts.Any())
            {
                var posts = _blogService.GetPosts();
                foreach (var blogPost in posts)
                {
                    var modifiedPost = modifiedPosts.FirstOrDefault(p =>
                        string.Equals(p, blogPost.GitHubPath, StringComparison.OrdinalIgnoreCase));
                    if (modifiedPost != null)
                    {
                        blogPost.LastUpdateTimeUtc = DateTime.UtcNow;
                    }
                }
            }

            return Ok("Local updated.");
        }
    }
}
