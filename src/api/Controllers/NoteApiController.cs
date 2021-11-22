using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Site.Jarvis;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Api.Controllers;

[Route("note")]
[ApiController]
public class NoteApiController : ControllerBase
{
    private readonly IFileRepository _fileRepository;

    public NoteApiController(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    [HttpGet]
    [Route("{link}")]
    public async Task<ActionResult<NoteRuntime>> GetNote([FromRoute] string link)
    {
        var note = await _fileRepository.GetNoteAsync(link);
        if (note == null)
        {
            return NotFound();
        }

        var noteRuntime = new NoteRuntime {Raw = note};
        noteRuntime.ExtractRuntimeData(new List<NoteTag>());
        return Ok(noteRuntime);
    }

    [HttpGet]
    public async Task<ActionResult<List<NoteRuntime>>> GetNotes([FromQuery] int? year = null)
    {
        var notes = await _fileRepository.GetNotesAsync(year);
        var result = new List<NoteRuntime>();
        foreach (var note in notes)
        {
            var noteRuntime = new NoteRuntime {Raw = note};
            noteRuntime.ExtractRuntimeData(new List<NoteTag>());
            result.Add(noteRuntime);
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> AddNote(Note note)
    {
        await _fileRepository.AddNoteAsync(note);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateNote(Note note)
    {
        await _fileRepository.UpdateNoteAsync(note);
        return Ok();
    }
}