using BizKeeper360.Interfaces;
using BizKeeper360.Models.DTO;
using BizKeeper360.Servises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace BizKeeper360.Controllers
{
    [Authorize]
    public class ItemController : BaseController<ItemController>
    {
        private readonly IItemService _itemService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HtmlValidator _htmlValidator;

        public ItemController(IItemService itemService, UserManager<IdentityUser> userManager,
            IStringLocalizer<ItemController> localizer, HtmlValidator htmlValidator)
            : base(localizer)
        {
            _itemService = itemService;
            _htmlValidator = htmlValidator;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ItemDTO model, string captcha)
        {
            string correctCaptcha = HttpContext.Session.GetString("Captcha");

            if (captcha != correctCaptcha)
            {
                ModelState.AddModelError("Captcha", "Неверная капча.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не найден.");
                return View(model);
            }

            if (!_htmlValidator.ValidateHtml(model.Description))
            {
                return View(model);
            }

            var item = await _itemService.CreateItemAsync(model, user.Id);
            return RedirectToAction("UserItems");
        }

        [HttpGet]
        public async Task<IActionResult> UserItems()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var items = await _itemService.GetUserItemsAsync(user.Id);
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _itemService.DeleteItemAsync(id, user.Id))
            {
                return NotFound();
            }

            return RedirectToAction("UserItems");
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _itemService.GetItemDetailsAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var item = await _itemService.GetItemDetailsAsync(id);
            if (item == null || item.UserId != user.Id)
            {
                return NotFound();
            }

            var model = new ItemDTO
            {
                Name = item.Name,
                Description = item.Description,
                ExpirationDate = item.ExpirationDate,
                Category = item.Category,
                Price = item.Price,
                Currency = item.Currency,
                ExistingImagePath = item.ImagePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ItemDTO model, string captcha)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var item = await _itemService.EditItemAsync(id, model, user.Id);
            if (item == null)
            {
                return NotFound();
            }

            if (!_htmlValidator.ValidateHtml(model.Description))
            {
                return View(model);
            }

            string correctCaptcha = HttpContext.Session.GetString("Captcha");

            if (captcha != correctCaptcha)
            {
                ModelState.AddModelError("Captcha", "Неверная капча.");
                return View(model);
            }

            return RedirectToAction("UserItems");
        }
    }
}