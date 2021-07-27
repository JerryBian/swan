using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Admin.HttpService;
using Laobian.Admin.Models;
using Laobian.Share;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers
{
    [Route("log")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ApiHttpService _apiHttpService;

        public LogController(ApiHttpService apiHttpService)
        {
            _apiHttpService = apiHttpService;
        }

        [Route("posts")]
        public async Task<IActionResult> GetLogsAsync([FromRoute]LaobianSite site, [FromRoute]string level)
        {
            var results = new ConcurrentDictionary<DateTime, List<string>>();
            var tasks = new List<Task>();
            return null;
        }
    }
}
