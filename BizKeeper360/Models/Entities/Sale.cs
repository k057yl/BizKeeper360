using BizKeeper360.Models.DTO;

namespace BizKeeper360.Models.Entities
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int ItemId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Profit { get; set; }
        public Item Item { get; set; }
    }
}
