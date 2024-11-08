using BizKeeper360.Data;
using BizKeeper360.Models.DTO;
using BizKeeper360.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BizKeeper360.Controllers
{
    [Authorize]
    public class ItemController : BaseController<ItemController>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IStringLocalizer<ItemController> localizer) : base(localizer)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(ItemDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    string? imagePath = null;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(model.ImageFile.FileName);
                        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }
                        var filePath = Path.Combine(uploads, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        imagePath = $"/images/{fileName}";
                    }

                    var item = new Item
                    {
                        Name = model.Name,
                        ExpirationDate = model.ExpirationDate,
                        ImagePath = imagePath,
                        Description = model.Description,
                        Rating = model.Rating,
                        Price = model.Price,
                        Currency = model.Currency,
                        UserId = user.Id,
                    };

                    _context.Items.Add(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "User not found.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserItems()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var items = await _context.Items
                .Where(i => i.UserId == user.Id)
                .ToListAsync();

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null || item.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("UserItems", "Item");
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Item/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id && i.UserId == _userManager.GetUserId(User));
            if (item == null)
            {
                return NotFound();
            }

            var model = new ItemDTO
            {
                Name = item.Name,
                Description = item.Description,
                ExpirationDate = item.ExpirationDate,
                Rating = item.Rating,
                Price = item.Price,
                Currency = item.Currency,
                ExistingImagePath = item.ImagePath // Путь к уже сохраненному изображению
            };

            return View(model);
        }

        // POST: Item/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ItemDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id && i.UserId == _userManager.GetUserId(User));
            if (item == null)
            {
                return NotFound();
            }

            item.Name = model.Name;
            item.Description = model.Description;
            item.ExpirationDate = model.ExpirationDate;
            item.Rating = model.Rating;
            item.Price = model.Price;
            item.Currency = model.Currency;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // Удаляем старое изображение, если оно существует
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Сохраняем новое изображение
                var fileName = Path.GetFileName(model.ImageFile.FileName);
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                item.ImagePath = $"/images/{fileName}";
            }

            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("UserItems");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsSold(int itemId, decimal salePrice)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
            {
                return NotFound();
            }

            // Устанавливаем флаг "продано" для предмета
            item.IsSold = true;

            // Создаем запись о продаже
            var sale = new Sale
            {
                ItemId = item.ItemId, // Ссылка на оригинальный предмет
                Name = item.Name,
                Description = item.Description,
                SalePrice = salePrice,
                SaleDate = DateTime.UtcNow,
                Currency = item.Currency
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sales()//*****************
        {
            var sales = await _context.Sales.Include(s => s.Item).ToListAsync();
            return View(sales);
        }
    }
}