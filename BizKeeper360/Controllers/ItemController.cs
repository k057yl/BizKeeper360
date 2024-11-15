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

                    var expirationDate = model.ExpirationDate ?? DateTime.MaxValue;

                    var item = new Item
                    {
                        Name = model.Name,
                        CreationDate = DateTime.Now,
                        ExpirationDate = expirationDate,
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
                ExistingImagePath = item.ImagePath
            };

            return View(model);
        }

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
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

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

        public async Task<IActionResult> SellItem(int itemId, decimal salePrice, decimal profit)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == itemId);
            if (item == null || item.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            string imagePath = item.ImagePath ?? "/images/Logo_v1.png";

            var sale = new Sale
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Description = item.Description,
                SaleDate = DateTime.Now,
                SalePrice = salePrice,
                Profit = salePrice - item.Price,
                Currency = item.Currency,
                ItemIsDeleted = item.IsDeleted,
                ItemImagePath = imagePath
            };

            _context.Sales.Add(sale);
            item.IsSold = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Sales");
        }

        public async Task<IActionResult> Sales(DateTime? startDate, DateTime? endDate)
        {
            var user = await _userManager.GetUserAsync(User);

            var salesQuery = _context.Sales
                .Include(s => s.Item)
                .Where(s => s.Item.UserId == user.Id || s.Item == null);

            if (startDate.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.SaleDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.SaleDate <= endDate.Value);
            }

            var sales = await salesQuery.ToListAsync();

            // Передача выбранных дат в представление для отображения в фильтре
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(sales);
        }

        /*
        public async Task<IActionResult> Sales()
        {
            var user = await _userManager.GetUserAsync(User);
            var sales = await _context.Sales
                .Include(s => s.Item)
                .Where(s => s.Item.UserId == user.Id || s.Item == null)
                .ToListAsync();

            return View(sales);
        }
        */
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.Include(i => i.Sales).FirstOrDefaultAsync(i => i.ItemId == id);
            if (item == null || item.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            item.IsDeleted = true;

            foreach (var sale in item.Sales)
            {
                sale.ItemIsDeleted = true;
            }

            _context.Items.Update(item);
            _context.Sales.UpdateRange(item.Sales);
            await _context.SaveChangesAsync();

            return RedirectToAction("UserItems", "Item");
        } 
    }
}