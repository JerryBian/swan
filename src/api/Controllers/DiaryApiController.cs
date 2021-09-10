using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share.Site.Jarvis;

namespace Laobian.Api.Controllers
{
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
        public async Task<ActionResult<Diary>> GetDiary([FromRoute]DateTime date)
        {
            var diary = await _fileRepository.GetDiaryAsync(date);
            if (diary == null)
            {
                return NotFound();
            }

            return Ok(diary);
        }

        [HttpGet]
        public async Task<ActionResult<List<Diary>>> GetDiary()
        {
            var diaries = await _fileRepository.GetDiariesAsync();
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
}
