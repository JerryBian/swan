using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Laobian.Blog.Models;
using Laobian.Share;
using Laobian.Share.Log;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [Route("/login")]
        public IActionResult Login(string r = null)
        {
            ViewData[ViewDataConstant.ReturnUrl] = r;
            ViewData[ViewDataConstant.Title] = "登录";
            ViewData[ViewDataConstant.VisibleToSearchEngine] = false;
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(string userName, string password, string r = null)
        {
            if (userName == Global.Config.Common.AdminUserName && password == Global.Config.Common.AdminPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim("user", userName),
                    new Claim("role", "admin")
                };

                await HttpContext.SignInAsync(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                        "user", "role")));

                if (string.IsNullOrEmpty(r))
                {
                    r = "/";
                }

                _logger.LogInformation($"Login successfully, user={userName}.");
                return Redirect(r);
            }

            _logger.LogWarning(LogMessageHelper.Format($"Login failed. User Name = {userName}, Password = {password}", HttpContext));
            return Redirect("/");
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation(LogMessageHelper.Format("Logout successfully.", HttpContext));
            return Redirect("/");
        }
    }
}