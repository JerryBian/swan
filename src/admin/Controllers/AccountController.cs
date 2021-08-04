using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly AdminConfig _adminConfig;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, IOptions<AdminConfig> options)
        {
            _logger = logger;
            _adminConfig = options.Value;
        }

        [HttpGet]
        [Route("/login")]
        public IActionResult Login([FromQuery]string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            if (userName == _adminConfig.AdminName && password == _adminConfig.AdminPassword)
            {
                var claims = new List<Claim>
                {
                    new("user", userName),
                    new("role", "admin")
                };

                var authProperty = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    IsPersistent = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                };
                await HttpContext.SignInAsync(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                        "user", "role")), authProperty);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = "/";
                }

                _logger.LogInformation($"Login successfully, user={userName}.");
                return Redirect(returnUrl);
            }

            _logger.LogWarning($"Login failed. User Name = {userName}, Password = {password}");
            return Redirect("/");
        }

        [Route("/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Logout successfully.");
            return Redirect("/");
        }
    }
}