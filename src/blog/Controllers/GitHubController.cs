using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Laobian.Blog.Data;
using Laobian.Blog.GitHub;
using Laobian.Blog.HttpService;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("github")]
    public class GitHubController : ControllerBase
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly BlogOption _blogOption;
        private readonly ILogger<GitHubController> _logger;
        private readonly ISystemData _systemData;

        public GitHubController(
            IOptions<BlogOption> blogConfig,
            ApiHttpService apiHttpService,
            ILogger<GitHubController> logger,
            ISystemData systemData)
        {
            _logger = logger;
            _blogOption = blogConfig.Value;
            _apiHttpService = apiHttpService;
            _systemData = systemData;
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
                //_logger.LogWarning(LogMessageHelper.Format("Headers are not completed.", HttpContext));
                return BadRequest("Invalid Request.");
            }

            if (!StringUtil.EqualsIgnoreCase("push", Request.Headers[eventHeader]))
            {
                //_logger.LogWarning(LogMessageHelper.Format($"Invalid github event {Request.Headers[eventHeader]}"));
                return BadRequest("Only support push event.");
            }

            var signature = Request.Headers[signatureHeader].ToString();
            if (!signature.StartsWith("sha1=", StringComparison.OrdinalIgnoreCase))
            {
                //_logger.LogWarning(LogMessageHelper.Format($"Invalid github signature {signature}", HttpContext));
                return BadRequest("Invalid signature.");
            }

            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                signature = signature.Substring("sha1=".Length);
                var secret = Encoding.UTF8.GetBytes(_blogOption.GitHubHookSecret);
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
                        //_logger.LogWarning(LogMessageHelper.Format($"Invalid github signature {signature}, {hashStr}"));
                        return BadRequest("Invalid signature.");
                    }
                }

                var payload = JsonUtil.Deserialize<GitHubPayload>(body);
                if (payload.Commits.Any(c =>
                    StringUtil.EqualsIgnoreCase(_blogOption.AdminEmail, c.Author.Email) &&
                    StringUtil.EqualsIgnoreCase(_blogOption.AdminUserName, c.Author.User)))
                {
                    //_logger.LogInformation(LogMessageHelper.Format("Got request from server, no need to refresh."));
                    return Ok("No need to refresh.");
                }

                var modifiedPosts = payload.Commits.SelectMany(c => c.Modified).Distinct().ToList();
                var addedPosts = payload.Commits.SelectMany(c => c.Added).Distinct().ToList();
#pragma warning disable 4014
                Task.Run(async () => // Make GitHub hook return fast.
#pragma warning restore 4014
                {
                    try
                    {
                        await _apiHttpService.PersistentAsync("GitHub Hook");
                        await _systemData.LoadAsync();
                        //_apiHttpService.
                        //var messages = await _blogService.GitHookAsync(modifiedPosts.Concat(addedPosts).ToList());
                        //if (string.IsNullOrEmpty(messages))
                        //{
                        //    _logger.LogInformation("Completed GitHub Hook with no warnings/errors. Great!");
                        //    await _blogAlertService.AlertEventAsync(
                        //        "<p>Completed GitHub Hook with no warnings/errors. Great!</p>");
                        //}
                        //else
                        //{
                        //    _logger.LogWarning($"Completed GitHub Hook with some warnings/errors.{Environment.NewLine}{messages}");
                        //    await _blogAlertService.AlertEventAsync(
                        //        $"<p>Completed GitHub Hook with some warnings/errors.</p><div><pre>{messages}</pre></div>");
                        //}
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed GitHub Hook with exception.");
                        //await _blogAlertService.AlertEventAsync(
                        //    $"<p>Failed GitHub Hook with exception.</p>", ex);
                    }
                });

                //_logger.LogInformation(LogMessageHelper.Format("GitHub Hook executed completed(not really)."));
                return Ok("Local updated.");
            }
        }
    }
}