using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share.Extension;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Admin.Controllers
{
    [Route("log")]
    public class LogController : Controller
    {
        private readonly ApiSiteHttpClient _apiSiteHttpClient;

        public LogController(ApiSiteHttpClient apiSiteHttpClient)
        {
            _apiSiteHttpClient = apiSiteHttpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("/log")]
        public async Task<IActionResult> GetLogs([FromQuery] string site, [FromQuery] int minLevel,
            [FromQuery] int days)
        {
            var logs = await _apiSiteHttpClient.GetLogsAsync(site, days);
            logs = logs.Where(x => (int) x.Level >= minLevel).OrderByDescending(x => x.TimeStamp).ToList();

            var sb = new StringBuilder();
            foreach (var log in logs)
            {
                var id = StringUtil.GenerateRandom();
                var theme = "info";
                if (log.Level == LogLevel.Warning)
                {
                    theme = "warning";
                }

                if (log.Level == LogLevel.Critical || log.Level == LogLevel.Error)
                {
                    theme = "danger";
                }

                var details = string.IsNullOrEmpty(log.Exception)
                    ? string.Empty
                    : $"<p><button class=\"btn btn-danger\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#details-{id}\" aria-expanded=\"false\">Show exception details</button></p>" +
                      $"<div class=\"collapse\" id=\"details-{id}\">" +
                      $"<pre>{log.Exception}</pre></div>";
                var html =
                    $"<div class=\"list-group-item list-group-item-{theme}\"><div class=\"d-flex w-100 justify-content-between\">" +
                    $"<h5 class=\"mb-1\">{log.LoggerName}</h5>" +
                    $"<small>{log.TimeStamp.ToDateAndTime()}</small></div>" +
                    $"<p class=\"mb-1\">{log.Message}</p>{details}</div>";
                sb.AppendLine(html);
            }

            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}