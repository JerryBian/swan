using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog;
using Laobian.Share.Git;
using Laobian.Share.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("github")]
    public class GitHubController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<GitHubController> _logger;

        public GitHubController(
            IBlogService blogService,
            ILogger<GitHubController> logger)
        {
            _logger = logger;
            _blogService = blogService;
        }

        // http://michaco.net/blog/HowToValidateGitHubWebhooksInCSharpWithASPNETCoreMVC
        [HttpPost]
        [Route("hook")]
        public async Task<IActionResult> Hook()
        {
            const string eventHeader = "X-GitHub-Event";
            const string signatureHeader = "X-Hub-Signature";
            const string deliveryHeader = "X-GitHub-Delivery";

            if (!Request.Headers.ContainsKey(eventHeader) ||
                !Request.Headers.ContainsKey(signatureHeader) ||
                !Request.Headers.ContainsKey(deliveryHeader))
            {
                _logger.LogWarning("Headers are not completed.");
                return BadRequest("Invalid Request.");
            }

            if (!CompareHelper.IgnoreCase("push", Request.Headers[eventHeader]))
            {
                _logger.LogWarning($"Invalid github event {Request.Headers[eventHeader]}");
                return BadRequest("Only support push event.");
            }

            var signature = Request.Headers[signatureHeader].ToString();
            if (!signature.StartsWith("sha1=", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"Invalid github signature {signature}");
                return BadRequest("Invalid signature.");
            }

            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                signature = signature.Substring("sha1=".Length);
                var secret = Encoding.UTF8.GetBytes(Global.Config.Blog.AssetGitHubHookSecret);
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
                        _logger.LogWarning($"Invalid github signature {signature}, {hashStr}");
                        return BadRequest("Invalid signature.");
                    }
                }

                var payload = SerializeHelper.FromJson<GitHubPayload>(body);
                if (payload.Commits.Any(c =>
                    CompareHelper.IgnoreCase(Global.Config.Blog.AssetGitCommitEmail, c.Author.Email) &&
                    CompareHelper.IgnoreCase(Global.Config.Blog.AssetGitCommitUser, c.Author.User)))
                {
                    _logger.LogInformation("Got request from server, no need to refresh.");
                    return Ok("No need to refresh.");
                }

                var modifiedPosts = payload.Commits.SelectMany(c => c.Modified).Distinct().ToList();
                var addedPosts = payload.Commits.SelectMany(c => c.Added).Distinct().ToList();
#pragma warning disable 4014
                Task.Run(async () => // Make GitHub hook return fast.
#pragma warning restore 4014
                {
                    await _blogService.ReloadLocalAssetsAsync(true, false, addedPosts, modifiedPosts);
                });

                _logger.LogInformation("GitHub Hook executed completed.");
                return Ok("Local updated.");
            }
        }
    }
}