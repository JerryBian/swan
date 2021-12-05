using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Site.Jarvis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Laobian.Api.Controllers;

[Route("diary")]
[ApiController]
public class DiaryApiController : ControllerBase
{
    private readonly IFileRepository _fileRepository;

    public DiaryApiController(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    [HttpGet]
    [Route("{date}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<DiaryRuntime>> GetDiary([FromRoute] DateTime date)
    {
        var diary = await _fileRepository.GetDiaryAsync(date);
        if (diary == null)
        {
            return NotFound();
        }

        var diaryRuntime = new DiaryRuntime {Raw = diary};
        diaryRuntime.ExtractRuntimeData();
        return Ok(diaryRuntime);
    }

    [HttpGet]
    [Route("list")]
    public async Task<ActionResult<List<DateTime>>> ListDiaries([FromQuery] int? year = null,
        [FromQuery] int? month = null)
    {
        var diaries = await _fileRepository.ListDiariesAsync(year, month);
        return Ok(diaries);
    }

    [HttpPut]
    public async Task<IActionResult> AddDiary(Diary diary)
    {
        await _fileRepository.AddDiaryAsync(diary);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDiary(Diary diary)
    {
        await _fileRepository.UpdateDiaryAsync(diary);
        return Ok();
    }
}