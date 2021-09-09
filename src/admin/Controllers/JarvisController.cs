using System;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share.Extension;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers
{
    [Route("jarvis")]
    public class JarvisController : Controller
    {
        private readonly LaobianAdminOption _option;
        private readonly ApiSiteHttpClient _httpClient;

        public JarvisController(IOptions<LaobianAdminOption> option, ApiSiteHttpClient httpClient)
        {
            _httpClient = httpClient;
            _option = option.Value;
        }

        [HttpGet]
        [Route("diary/add")]
        public async Task<IActionResult> AddDiary([FromQuery]DateTime date)
        {
            if (date == default)
            {
                date = DateTime.Now;
            }

            var item = await _httpClient.GetDiaryAsync(date);
            if (item != null)
            {
                return Redirect($"/jarvis/diary/update/{date.ToDate()}");
            }

            ViewData["Date"] = date.ToDate();
            return View();
        }

        [HttpPost]
        [Route("diary/add")]
        public async Task<IActionResult> AddDiary([FromForm] Diary diary)
        {
            if (diary.Date == default)
            {
                diary.Date = DateTime.Now;
            }

            await _httpClient.AddDiaryAsync(diary);
            return Redirect(diary.GetFullPath(_option));
        }

        [HttpGet]
        [Route("diary/update/{date}")]
        public async Task<IActionResult> UpdateDiary(DateTime date)
        {
            var item = await _httpClient.GetDiaryAsync(date);
            if (item == null)
            {
                return Redirect($"/jarvis/diary/add?date={date.ToDate()}");
            }

            return View(item);
        }

        [HttpPost]
        [Route("diary/update")]
        public async Task<IActionResult> UpdateDiary([FromForm] Diary diary)
        {
            return Redirect(diary.GetFullPath(_option));
        }
    }
}
