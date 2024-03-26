using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Swan.Core.Extension;
using Swan.Core.Option;
using Swan.Core.Store;
using System.Security.Claims;

namespace Swan.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SwanOption _option;
        private readonly ISwanStore _swanStore;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ILogger<AccountController> logger,
            ISwanStore swanStore,
            IOptions<SwanOption> options)
        {
            _logger = logger;
            _swanStore = swanStore;
            _option = options.Value;
        }

        [OutputCache]
        [HttpGet("/login")]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult Login([FromQuery] string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(
            [FromForm] string userName,
            [FromForm] string password,
            [FromQuery] string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            string ip = HttpContext.GetIpAddress();
            if (userName == _option.AdminUserName && password == _option.AdminPassword)
            {
                List<Claim> claims =
                [
                    new("user", userName),
                    new("role", "admin")
                ];

                AuthenticationProperties authProperty = new()
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
                else if (!Url.IsLocalUrl(returnUrl))
                {
                    _logger.LogWarning($"Invalid Return Url: {returnUrl}");
                    returnUrl = "/";
                }

                _logger.LogInformation($"Login successfully, user={userName}.");
                return Redirect(returnUrl);
            }

            _logger.LogWarning(
                $"Login failed. User Name = {userName}, Password = {password}. IP: {ip}. Headers: {string.Join(", ", Request.Headers.Select(x => $"{x.Key} -> {x.Value}"))}");
            await _swanStore.AddBlacklistAsync(ip);
            return Redirect("/");
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Logout successfully.");
            return Redirect("/");
        }
    }
}
