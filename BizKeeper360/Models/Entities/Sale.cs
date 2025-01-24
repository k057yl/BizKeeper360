using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizKeeper360.Models.Entities
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; }

        public int? ItemId { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [NotMapped]
        public DateTime? ItemCreationDate => Item?.CreationDate;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Profit { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }

        [NotMapped]
        public string? ItemName => Item != null ? Item.Name : Name;

        public bool ItemIsDeleted { get; set; }

        [MaxLength(500)]
        public string? ItemImagePath { get; set; }
    }
}
