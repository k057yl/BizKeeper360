using BizKeeper360.Data;
using BizKeeper360.Interfaces;
using BizKeeper360.Models.DTO;
using BizKeeper360.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizKeeper360.Servises
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        public ItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Item> CreateItemAsync(ItemDTO model, string userId)
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

            var utcExpirationDate = model.ExpirationDate.HasValue
                ? DateTime.SpecifyKind(model.ExpirationDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(new DateTime(2099, 12, 31), DateTimeKind.Utc);

            var item = new Item
            {
                Name = model.Name,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = utcExpirationDate,
                ImagePath = imagePath,
                Description = model.Description,
                Category = model.Category,
                Price = model.Price,
                Currency = model.Currency,
                UserId = userId,
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<Item> EditItemAsync(int id, ItemDTO model, string userId)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id && i.UserId == userId);
            if (item == null)
            {
                return null;
            }

            item.Name = model.Name;
            item.Description = model.Description;
            item.ExpirationDate = model.ExpirationDate.HasValue ? DateTime.SpecifyKind(model.ExpirationDate.Value, DateTimeKind.Utc) : (DateTime?)null;
            item.Category = model.Category;
            item.Price = model.Price;
            item.Currency = model.Currency;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImagePath.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
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

            return item;
        }

        public async Task<bool> DeleteItemAsync(int id, string userId)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null || item.UserId != userId)
            {
                return false;
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Item> GetItemDetailsAsync(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.ItemId == id);
        }

        public async Task<List<Item>> GetUserItemsAsync(string userId)
        {
            return await _context.Items.Where(i => i.UserId == userId).ToListAsync();
        }

        public async Task<Sale> SellItemAsync(int itemId, decimal salePrice, decimal profit, string userId)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == itemId && i.UserId == userId);
            if (item == null)
            {
                return null;
            }

            string imagePath = item.ImagePath ?? "/images/Logo_v1.png";

            var sale = new Sale
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Description = item.Description,
                SaleDate = DateTime.UtcNow,
                SalePrice = salePrice,
                Profit = salePrice - item.Price,
                Currency = item.Currency,
                ItemIsDeleted = item.IsDeleted,
                ItemImagePath = imagePath
            };

            _context.Sales.Add(sale);
            item.IsSold = true;
            await _context.SaveChangesAsync();

            return sale;
        }

        public async Task<List<Sale>> GetSalesAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            var salesQuery = _context.Sales.Include(s => s.Item).Where(s => s.Item.UserId == userId || s.Item == null);

            if (startDate.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.SaleDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                salesQuery = salesQuery.Where(s => s.SaleDate <= endDate.Value);
            }

            return await salesQuery.ToListAsync();
        }
    }
}
