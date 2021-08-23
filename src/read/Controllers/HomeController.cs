using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Read.HttpClients;
using Laobian.Read.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Read.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiSiteHttpClient _apiSiteHttpClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApiSiteHttpClient apiSiteHttpClient)
        {
            _logger = logger;
            _apiSiteHttpClient = apiSiteHttpClient;
        }

        public async Task<IActionResult> Index()
        {
            var readItems = await _apiSiteHttpClient.GetAllReadItemsAsync();
            var model = readItems.GroupBy(x => x.StartTime.Year).Select(x =>
            {
                var item = new GroupedReadItems
                {
                    Title = $"{x.Key} nian",
                    Id = x.Key.ToString(),
                    Count = x.Count()
                };
                item.Items.AddRange(x.OrderByDescending(y => y.StartTime));
                return item;
            });
            return View(model.OrderByDescending(x => x.Id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}