using Laobian.Lib.Option;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;

namespace Laobian.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly LaobianOption _option;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger, IOptions<LaobianOption> options)
    {
        _logger = logger;
        _option = options.Value;
    }

    [HttpGet]
    [Route("/login")]
    public IActionResult Login([FromQuery] string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [Route("/login")]
    public async Task<IActionResult> Login([FromForm] string userName, [FromForm] string password,
        [FromQuery] string returnUrl = null)
    {
        if (userName == _option.AdminUserName && password == _option.AdminPassword)
        {
            List<Claim> claims = new()
            {
                new("user", userName),
                new("role", "admin")
            };

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
                Uri uri = new(returnUrl);
                if (!uri.Host.EndsWith("localhost") && !uri.Host.EndsWith(".laobian.me"))
                {
                    _logger.LogWarning($"Invalid Return Url: {returnUrl}");
                    returnUrl = "/";
                }
            }

            _logger.LogInformation($"Login successfully, user={userName}.");
            return Redirect(returnUrl);
        }

        _logger.LogWarning(
            $"Login failed. User Name = {userName}, Password = {password}. IP: {HttpContext.Connection.RemoteIpAddress}, User Agent: {Request.Headers[HeaderNames.UserAgent]}");
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