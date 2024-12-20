﻿using Microsoft.AspNetCore.Identity;

namespace BizKeeper360.Models.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Rating { get; set; }
        public string Currency { get; set; }
        public bool IsSold { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Sale> Sales { get; set; }

        // Convert DateTime to UTC before saving
        public DateTime GetUtcCreationDate() => DateTime.SpecifyKind(CreationDate, DateTimeKind.Utc);
        public DateTime? GetUtcExpirationDate() => ExpirationDate.HasValue ? DateTime.SpecifyKind(ExpirationDate.Value, DateTimeKind.Utc) : (DateTime?)null;
    }
}
