using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Laobian.Share.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, IOptions<AppConfig> appConfig)
        {
            _logger = logger;
            _appConfig = appConfig.Value;
        }

        [Route("/login")]
        public IActionResult Login(string r = null)
        {
            ViewData["ReturnUrl"] = r;
            ViewData["Title"] = "登录";
            ViewData["Robots"] = "noindex, nofollow";
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(string userName, string password, string r = null)
        {
            if (userName == _appConfig.Common.AdminUserName && password == _appConfig.Common.AdminPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim("user", userName),
                    new Claim("role", "admin")
                };

                await HttpContext.SignInAsync(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "user", "role")));

                if (string.IsNullOrEmpty(r))
                {
                    r = "/";
                }

                _logger.LogInformation("Login successfully.");
                return Redirect(r);
            }

            _logger.LogWarning("Login failed. User Name = {UserName}, Password = {Password}", userName, password);
            return Redirect("/");
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Logout successfully.");
            return Redirect("/");
        }
    }
}
