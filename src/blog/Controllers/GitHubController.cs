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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("github")]
    public class GitHubController : ControllerBase
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;
        private readonly ILogger<GitHubController> _logger;

        public GitHubController(IBlogService blogService, IOptions<AppConfig> appConfig, ILogger<GitHubController> logger)
        {
            _logger = logger;
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
                _logger.LogWarning("Headers are not completed.");
                return BadRequest("Invalid Request.");
            }

            if (!StringEqualsHelper.IgnoreCase("push", Request.Headers["X-GitHub-Event"]))
            {
                _logger.LogWarning("Invalid github event {Event}", Request.Headers["X-GitHub-Event"]);
                return BadRequest("Only support push event.");
            }

            var signature = Request.Headers["X-Hub-Signature"].ToString();
            if (!signature.StartsWith("sha1=", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid github signature {Signature}", signature);
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
                        _logger.LogWarning("Invalid github signature {Signature}, {HashString}", signature, hashStr);
                        return BadRequest("Invalid signature.");
                    }
                }

                var payload = SerializeHelper.FromJson<GitHubPayload>(body);
                if (payload.Commits.Any(c =>
                    StringEqualsHelper.IgnoreCase(_appConfig.AssetGitCommitEmail, c.Author.Email) &&
                    StringEqualsHelper.IgnoreCase(_appConfig.AssetGitCommitUser, c.Author.User)))
                {
                    _logger.LogInformation("Got request from server, no need to refresh.");
                    return Ok("No need to refresh.");
                }

                var modifiedPosts = payload.Commits.SelectMany(c => c.Modified).Distinct().ToList();
                await _blogService.UpdateMemoryAssetsAsync();
                _logger.LogInformation("Local assets refreshed.");

                bool requireCommit = false;

                if (modifiedPosts.Any())
                {
                    var posts = _blogService.GetPosts();
                    foreach (var blogPost in posts)
                    {
                        var modifiedPost = modifiedPosts.FirstOrDefault(p =>
                            string.Equals(p, blogPost.GitHubPath, StringComparison.OrdinalIgnoreCase));
                        if (modifiedPost != null)
                        {
                            requireCommit = true;
                            blogPost.LastUpdateTimeUtc = DateTime.UtcNow;
                        }
                    }
                }

                if (requireCommit || payload.Commits.SelectMany(c => c.Added).Any())
                {
                    await _blogService.UpdateCloudAssetsAsync();
                    _logger.LogInformation("Cloud assets updated.");
                }

                return Ok("Local updated.");
            }
        }
    }
}
