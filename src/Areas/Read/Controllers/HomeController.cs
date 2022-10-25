﻿using Laobian.Areas.Read.Models;
using Laobian.Lib;
using Laobian.Lib.Service;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Areas.Read.Controllers
{
    [Area(Constants.AreaRead)]
    public class HomeController : Controller
    {
        private readonly IReadService _readService;

        public HomeController(IReadService readService)
        {
            _readService = readService;
        }

        [ResponseCache(CacheProfileName = Constants.CacheProfileServerShort)]
        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated == true;
            List<Lib.Model.ReadItemView> items = await _readService.GetAllAsync();
            if (!isAuthenticated)
            {
                items = items.Where(x => x.Raw.IsPublic).ToList();
            }

            List<ReadIndexViewModel> model = new();
            foreach (IGrouping<int, Lib.Model.ReadItemView> item in items.GroupBy(x => x.Raw.CreateTime.Year).OrderByDescending(x => x.Key))
            {
                ReadIndexViewModel vm = new()
                {
                    Title = item.Key.ToString(),
                    Id = item.Key.ToString(),
                    Count = item.Count(),
                    Items = item.OrderByDescending(x => x.Raw.CreateTime).ToList()
                };
                model.Add(vm);
            }

            ViewData["Title"] = $"阅读";
            ViewData["DatePublished"] = items.Min(x => x.Raw.CreateTime);
            ViewData["DateModified"] = items.Max(x => x.Raw.LastUpdateTime);
            return View(model);
        }
    }
}