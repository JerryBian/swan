using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Helper;
using Laobian.Share.Infrastructure.Git;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("github")]
    public class GitHubController : ControllerBase
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public GitHubController(IBlogService blogService, IOptions<AppConfig> appConfig)
        {
            _blogService = blogService;
            _appConfig = appConfig.Value;
        }

        // http://michaco.net/blog/HowToValidateGitHubWebhooksInCSharpWithASPNETCoreMVC
        [HttpPost]
        [Route("hook")]
        public async Task<IActionResult> Hook()
        {
            if (!Request.Headers.ContainsKey("X-GitHub-Event") ||
                !Request.Headers.ContainsKey("X-Hub-Signature") ||
                !Request.Headers.ContainsKey("X-GitHub-Delivery"))
            {
                return BadRequest("Invalid Request.");
            }

            if (!StringEqualsHelper.EqualsIgnoreCase("push", Request.Headers["X-GitHub-Event"]))
            {
                return BadRequest("Only support push event.");
            }

            var signature = Request.Headers["X-Hub-Signature"].ToString();
            if (!signature.StartsWith("sha1=", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid signature.");
            }

            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                signature = signature.Substring("sha1=".Length);
                var secret = Encoding.UTF8.GetBytes(_appConfig.AssetGitHubHookSecret);
                var bodyBytes = Encoding.UTF8.GetBytes(body);

                using (var hmacSha1 = new HMACSHA1(secret))
                {
                    var hash = hmacSha1.ComputeHash(bodyBytes);
                    var builder = new StringBuilder(hash.Length * 2);
                    foreach (var b in hash)
                    {
                        builder.AppendFormat("{0:x2}", b);
                    }

                    var hashStr = builder.ToString();

                    if (!hashStr.Equals(signature))
                    {
                        return BadRequest("Invalid signature.");
                    }
                }

                var payload = SerializeHelper.FromJson<GitHubPayload>(body);
                await _blogService.UpdateMemoryAssetsAsync();

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

                await _blogService.UpdateCloudAssetsAsync();
                return Ok("Local updated.");
            }
        }
    }
}
