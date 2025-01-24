using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BizKeeper360.Models.Enums;

namespace BizKeeper360.Models.Entities
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [MaxLength(500)]
        public string ImagePath { get; set; }

        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public Category Category { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; }

        [Required]
        public bool IsSold { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public ICollection<Sale> Sales { get; set; }

        // Convert DateTime to UTC before saving
        public DateTime GetUtcCreationDate() => DateTime.SpecifyKind(CreationDate, DateTimeKind.Utc);
        public DateTime? GetUtcExpirationDate() => ExpirationDate.HasValue ? DateTime.SpecifyKind(ExpirationDate.Value, DateTimeKind.Utc) : (DateTime?)null;
    }
}
