using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.BlogEngine;
using Laobian.Share.Config;
using Laobian.Share.Helper;
using Laobian.Share.Infrastructure.Email;
using Laobian.Share.Infrastructure.Git;
using Laobian.Share.Log;
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
        private readonly IEmailClient _emailClient;
        private readonly IBlogService _blogService;
        private readonly ILogService _logService;

        public GitHubController(IEmailClient emailClient, IBlogService blogService, IOptions<AppConfig> appConfig, ILogService logService)
        {
            _logService = logService;
            _emailClient = emailClient;
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
                await _logService.LogWarning("Headers are not completed.");
                return BadRequest("Invalid Request.");
            }

            if (!StringEqualsHelper.IgnoreCase("push", Request.Headers["X-GitHub-Event"]))
            {
                await _logService.LogWarning($"Invalid github event {Request.Headers["X-GitHub-Event"]}");
                return BadRequest("Only support push event.");
            }

            var signature = Request.Headers["X-Hub-Signature"].ToString();
            if (!signature.StartsWith("sha1=", StringComparison.OrdinalIgnoreCase))
            {
                await _logService.LogWarning($"Invalid github signature {signature}");
                return BadRequest("Invalid signature.");
            }

            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                signature = signature.Substring("sha1=".Length);
                var secret = Encoding.UTF8.GetBytes(_appConfig.Blog.AssetGitHubHookSecret);
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
                        await _logService.LogWarning($"Invalid github signature {signature}, {hashStr}");
                        return BadRequest("Invalid signature.");
                    }
                }

                var payload = SerializeHelper.FromJson<GitHubPayload>(body);
                if (payload.Commits.Any(c =>
                    StringEqualsHelper.IgnoreCase(_appConfig.Blog.AssetGitCommitEmail, c.Author.Email) &&
                    StringEqualsHelper.IgnoreCase(_appConfig.Blog.AssetGitCommitUser, c.Author.User)))
                {
                    await _logService.LogInformation("Got request from server, no need to refresh.");
                    return Ok("No need to refresh.");
                }

                var modifiedPosts = payload.Commits.SelectMany(c => c.Modified).Distinct().ToList();
                await _blogService.UpdateMemoryAssetsAsync();
                await _logService.LogInformation("Local assets refreshed.");

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
                await _logService.LogInformation("Cloud assets updated.");

                var email = new StringBuilder();
                email.AppendLine($"<h3>ADDED POSTS</h3><ul>");
                foreach (var post in payload.Commits.SelectMany(c => c.Added).Distinct())
                {
                    email.AppendLine($"<li>{post}</li>");
                }

                email.AppendLine("</ul><h3>MODIFIED POSTS</h3><ul>");
                foreach (var post in modifiedPosts)
                {
                    email.AppendLine($"<li>{post}</li>");
                }

                email.AppendLine("</ul>");

                await _emailClient.SendAsync(
                    _appConfig.Common.ReportSenderName,
                    _appConfig.Common.ReportSenderEmail,
                    _appConfig.Common.AdminEnglishName,
                    _appConfig.Common.AdminEmail,
                    "GITHUB HOOK COMPLETED",
                    $"<p>GitHub hook executed completed.</p>{email}");

                return Ok("Local updated.");
            }
        }
    }
}
