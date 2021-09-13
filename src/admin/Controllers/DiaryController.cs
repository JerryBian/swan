using System;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share.Extension;
using Laobian.Share.Site.Jarvis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers
{
    [Route("diary")]
    public class DiaryController : Controller
    {
        private readonly ApiSiteHttpClient _httpClient;
        private readonly LaobianAdminOption _option;

        public DiaryController(IOptions<LaobianAdminOption> option, ApiSiteHttpClient httpClient)
        {
            _httpClient = httpClient;
            _option = option.Value;
        }

        [HttpGet]
        [Route("add")]
        public IActionResult AddDiary([FromQuery] DateTime date)
        {
            if (date == default)
            {
                date = DateTime.Now;
            }

            ViewData["Date"] = date.ToDate();
            return View();
        }

        [HttpPost]
        [Route("add")]
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
        [Route("update/{date}")]
        public async Task<IActionResult> UpdateDiary(DateTime date)
        {
            var item = await _httpClient.GetDiaryAsync(date);
            if (item == null)
            {
                return Redirect($"/diary/add?date={date.ToDate()}");
            }

            return View(item);
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateDiary([FromForm] Diary diary)
        {
            await _httpClient.UpdateDiaryAsync(diary);
            return Redirect(diary.GetFullPath(_option));
        }
    }
}