using System;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Jarvis.HttpClients;
using Laobian.Jarvis.Models;
using Laobian.Share.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.Controllers
{
    [Route("diary")]
    public class DiaryController : Controller
    {
        private readonly ApiSiteHttpClient _httpClient;
        private readonly JarvisOption _option;

        public DiaryController(ApiSiteHttpClient httpClient, IOptions<JarvisOption> option)
        {
            _option = option.Value;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var diaries = await _httpClient.ListDiariesAsync();
            return View(diaries);
        }

        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> List([FromRoute] int year)
        {
            var diaries = await _httpClient.ListDiariesAsync(year);
            return View(diaries);
        }

        [HttpGet]
        [Route("{year}/{date}.html")]
        public async Task<IActionResult> Detail([FromRoute] int year, [FromRoute] DateTime date)
        {
            var diary = await _httpClient.GetDiaryAsync(date);
            if (diary == null)
            {
                return Redirect($"{_option.AdminRemoteEndpoint}/diary/add/{date.ToDate()}");
            }

            if (diary.Raw.Date.Year != year)
            {
                return NotFound();
            }

            var model = new DiaryViewModel
            {
                Current = diary
            };

            var diaries = (await _httpClient.ListDiariesAsync(year)).OrderByDescending(x => x).ToList();
            var prevDate = diaries.FirstOrDefault(x => x < date);
            if (prevDate != default)
            {
                model.Prev = await _httpClient.GetDiaryAsync(prevDate);
            }

            var nextDate = diaries.LastOrDefault(x => x > date);
            if (nextDate != default)
            {
                model.Next = await _httpClient.GetDiaryAsync(nextDate);
            }

            return View(model);
        }
    }
}