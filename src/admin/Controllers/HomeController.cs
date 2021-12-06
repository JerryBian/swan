using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers;

public class HomeController : Controller
{
    private readonly ApiSiteHttpClient _apiSiteHttpClient;

    public HomeController(ApiSiteHttpClient apiSiteHttpClient)
    {
        _apiSiteHttpClient = apiSiteHttpClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    [HttpPost]
    [Route("persistent")]
    public async Task<bool> PersistentAsync()
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var message = await reader.ReadToEndAsync();
        return await _apiSiteHttpClient.PersistentAsync(message);
    }
}