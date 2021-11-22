using System.Threading.Tasks;
using Laobian.Jarvis.HttpClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.Controllers;

[Route("note")]
public class NoteController : Controller
{
    private readonly ApiSiteHttpClient _httpClient;
    private readonly JarvisOptions _options;

    public NoteController(ApiSiteHttpClient httpClient, IOptions<JarvisOptions> option)
    {
        _options = option.Value;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        var notes = await _httpClient.GetNotesAsync();
        return View(notes);
    }

    [HttpGet]
    [Route("{link}.html")]
    public async Task<IActionResult> Detail([FromRoute] string link)
    {
        var note = await _httpClient.GetNoteAsync(link);
        if (note == null)
        {
            return NotFound();
        }

        return View(note);
    }
}