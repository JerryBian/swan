using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Laobian.Share.Config;
using Laobian.Share.Log;
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
        private readonly ILogService _logService;

        public AccountController(ILogService logService, IOptions<AppConfig> appConfig)
        {
            _logService = logService;
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

                await _logService.LogInformation("Login successfully.");
                return Redirect(r);
            }

            await _logService.LogWarning($"Login failed. User Name = {userName}, Password = {password}");
            return Redirect("/");
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await _logService.LogInformation("Logout successfully.");
            return Redirect("/");
        }
    }
}
