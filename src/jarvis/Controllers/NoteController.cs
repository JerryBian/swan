using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Jarvis.HttpClients;
using Laobian.Share.Site.Jarvis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Jarvis.Controllers
{
    [Route("note")]
    public class NoteController : Controller
    {
        private readonly ApiSiteHttpClient _httpClient;
        private readonly JarvisOption _option;

        public NoteController(ApiSiteHttpClient httpClient, IOptions<JarvisOption> option)
        {
            _option = option.Value;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var notes = await _httpClient.GetNotesAsync();
            return View(notes);
        }

        [HttpGet]
        [Route("{year}/{link}.html")]
        public async Task<IActionResult> Detail([FromRoute] int year, [FromRoute] string link)
        {
            var note = await _httpClient.GetNoteAsync(link);
            if (note == null || note.Raw.CreateTime.Year != year)
            {
                return NotFound();
            }

            return View(note);
        }
    }
}
