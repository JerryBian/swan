using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Jarvis.HttpClients;

namespace Laobian.Jarvis.Controllers
{
    public class DiaryController : Controller
    {
        private readonly ApiSiteHttpClient _httpClient;

        public DiaryController(ApiSiteHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var diaries = await _httpClient.GetDiariesAsync();
            return View(diaries);
        }
    }
}
