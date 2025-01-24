using BizKeeper360.Models.DTO;
using BizKeeper360.Models.Entities;

namespace BizKeeper360.Interfaces
{
    public interface IItemService
    {
        Task<Item> CreateItemAsync(ItemDTO model, string userId);
        Task<Item> EditItemAsync(int id, ItemDTO model, string userId);
        Task<bool> DeleteItemAsync(int id, string userId);
        Task<Item> GetItemDetailsAsync(int id);
        Task<List<Item>> GetUserItemsAsync(string userId);
        Task<Sale> SellItemAsync(int itemId, decimal salePrice, decimal profit, string userId);
        Task<List<Sale>> GetSalesAsync(string userId, DateTime? startDate, DateTime? endDate);
    }
}
