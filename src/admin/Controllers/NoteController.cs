using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Admin.HttpClients;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers;

[Route("note")]
public class NoteController : Controller
{
    private readonly ApiSiteHttpClient _httpClient;
    private readonly AdminOptions _options;

    public NoteController(IOptions<AdminOptions> option, ApiSiteHttpClient httpClient)
    {
        _httpClient = httpClient;
        _options = option.Value;
    }

    [HttpGet]
    [Route("add")]
    public IActionResult AddNote()
    {
        return View(new List<NoteTag>());
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddNote([FromForm] Note note)
    {
        note.CreateTime = DateTime.Now;
        note.LastUpdateTime = DateTime.Now;
        note.Id = StringUtil.GenerateRandom();
        await _httpClient.AddNoteAsync(note);
        return Redirect($"{_options.JarvisRemoteEndpoint}{note.GetFullPath(_options)}");
    }

    [HttpGet]
    [Route("update/{link}")]
    public async Task<IActionResult> UpdateNote([FromRoute] string link)
    {
        var item = await _httpClient.GetNoteAsync(link);
        if (item == null)
        {
            return NotFound();
        }

        return View(item.Raw);
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> UpdateNote([FromForm] Note note)
    {
        await _httpClient.UpdateNoteAsync(note);
        return Redirect($"{_options.JarvisRemoteEndpoint}{note.GetFullPath(_options)}");
    }
}