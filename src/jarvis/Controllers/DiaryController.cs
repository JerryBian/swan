using System;
using System.Collections.Generic;
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
        [Route("{year}/{month}")]
        public async Task<IActionResult> ListMonth([FromRoute] int year, [FromRoute] int month)
        {
            var diaries = await _httpClient.ListDiariesAsync(year, month);
            return View(diaries.OrderByDescending(x => x).ToList());
        }

        [HttpGet]
        [Route("{year}")]
        public async Task<IActionResult> ListYear([FromRoute] int year)
        {
            var diaries = await _httpClient.ListDiariesAsync(year);
            var result = new Dictionary<int, List<DateTime>>();
            foreach (var item in diaries.GroupBy(x => x.Month).OrderByDescending(x => x.Key))
            {
                var list = new List<DateTime>();
                foreach (var d in item.OrderByDescending(x => x))
                {
                    list.Add(d);
                }

                result.Add(item.Key, list);
            }
            return View(result);
        }

        [HttpGet]
        [Route("{year}/{month}/{day}.html")]
        public async Task<IActionResult> Detail([FromRoute] int year, [FromRoute] int month, [FromRoute] int day)
        {
            var date = new DateTime(year, month, day);
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