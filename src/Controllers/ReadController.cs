using Microsoft.AspNetCore.Mvc;
using Swan.Core.Store;

namespace Swan.Controllers;

public class ReadController : Controller
{
    private readonly ISwanStore _swanStore;

    public ReadController(ISwanStore swanStore)
    {
        _swanStore = swanStore;
    }

    public async Task<IActionResult> Index()
    {
        var readItems = await _swanStore.GetReadItemsAsync(true);
        return View(readItems);
    }
}
