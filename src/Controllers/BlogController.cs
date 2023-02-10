using Microsoft.AspNetCore.Mvc;
using Swan.Core.Helper;
using Swan.Core.Model.Object;
using Swan.Core.Model;
using Swan.Core.Service;

namespace Swan.Controllers
{
    [Route("blog")]
    public class BlogController : Controller
    {
        private ILogger<BlogController> _logger;
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger)
        {
            _logger = logger;
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("tag")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _blogService.GetAllTagsAsync();
            return View("AllTags", tags);
        }

        [HttpGet("tag/{url}")]
        public async Task<IActionResult> GetTag([FromRoute]string url)
        {
            var tag = await _blogService.GetTagByUrlAsync(url);
            if(tag == null)
            {
                return NotFound();
            }

            return View("Tag", tag);
        }

        [Route("tag/add")]
        public IActionResult AddTag()
        {
            return View("AddTag");
        }

        [HttpPost("tag/add")]
        public async Task<IActionResult> AddTag([FromForm] BlogTagObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.CreateTagAsync(item);
                res.RedirectTo = "/blog/tag";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        [HttpGet("tag/{id}/edit")]
        public async Task<IActionResult> EditTag([FromRoute] string id)
        {
            var item = await _blogService.GetTagAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View("EditTag", item);
        }

        [HttpPut("tag/edit")]
        public async Task<IActionResult> EditTag([FromForm] BlogTagObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.UpdateTagAsync(item);
                res.RedirectTo = "/blog/tag";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Update read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        [HttpGet("series")]
        public async Task<IActionResult> GetSeries()
        {
            var series = await _blogService.GetAllSeriesAsync();
            return View("AllSeries", series);
        }

        [HttpGet("series/{url}")]
        public async Task<IActionResult> GetSeries([FromRoute] string url)
        {
            var series = await _blogService.GetSeriesByUrlAsync(url);
            if (series == null)
            {
                return NotFound();
            }

            return View("series", series);
        }

        [Route("series/add")]
        public IActionResult AddSeries()
        {
            return View("AddSeries");
        }

        [HttpPost("series/add")]
        public async Task<IActionResult> AddSeries([FromForm] BlogSeriesObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.CreateSeriesAsync(item);
                res.RedirectTo = "/blog/series";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Add new read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }

        [HttpGet("series/{id}/edit")]
        public async Task<IActionResult> EditSeries([FromRoute] string id)
        {
            var item = await _blogService.GetSeriesAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View("EditSeries", item);
        }

        [HttpPut("series/edit")]
        public async Task<IActionResult> EditSeries([FromForm] BlogSeriesObject item)
        {
            ApiResponse<object> res = new();
            try
            {
                await _blogService.UpdateSeriesAsync(item);
                res.RedirectTo = "/blog/series";
            }
            catch (Exception ex)
            {
                res.IsOk = false;
                res.Message = ex.Message;
                _logger.LogError(ex, $"Update read item failed => {JsonHelper.Serialize(item)}");
            }

            return Json(res);
        }
    }
}
