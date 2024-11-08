using BizKeeper360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace BizKeeper360.Controllers
{
    //[Authorize]
    public class HomeController : BaseController<HomeController>
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(IStringLocalizer<HomeController> localizer) : base(localizer)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
