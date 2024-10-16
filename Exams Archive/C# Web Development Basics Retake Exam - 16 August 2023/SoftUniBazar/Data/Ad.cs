namespace SoftUniBazar.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static SoftUniBazar.Constants.CommonConstants;

    public class Ad
    {
        [Key]
        [Comment("Ad identifier")]
        public int Id { get; set; }

        [Required]
        [StringLength(AdNameMaxLength, MinimumLength = AdNameMinLength, ErrorMessage = DefaultErrorMessage)]
        [Comment("Ad name")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(AdDescriptionMaxLength, MinimumLength = AdDescriptionMinLength, ErrorMessage = DefaultErrorMessage)]
        [Comment("Ad description")]
        public string Description { get; set; } = null!;

        [Required]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price")]
        [Comment("Ad price")]
        public decimal Price { get; set; }

        [Required]
        [Comment("Owner identifier")]
        public string OwnerId { get; set; } = null!;

        [ForeignKey(nameof(OwnerId))]
        public IdentityUser Owner { get; set; } = null!;

        [Required]
        [Comment("Ad image")]
        public string ImageUrl {  get; set; } = null!;

        [Required]
        [Comment("Ad's date of creation")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [Comment("Category identifier")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
