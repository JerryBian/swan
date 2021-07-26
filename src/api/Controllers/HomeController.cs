using Microsoft.AspNetCore.Mvc;

namespace Laobian.Api.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            return Problem();
        }
    }
}