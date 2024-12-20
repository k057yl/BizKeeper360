﻿using System.ComponentModel.DataAnnotations;

namespace BizKeeper360.Models.Entities
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int? ItemId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime? ItemCreationDate => Item?.CreationDate;
        public decimal SalePrice { get; set; }
        public string? Currency { get; set; }
        public decimal Profit { get; set; }
        public Item? Item { get; set; }

        public string? ItemName => Item != null ? Item.Name : Name;

        public bool ItemIsDeleted { get; set; }

        public string? ItemImagePath { get; set; }
    }
}
