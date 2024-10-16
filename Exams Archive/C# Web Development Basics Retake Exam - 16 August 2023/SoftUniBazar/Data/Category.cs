namespace SoftUniBazar.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using static SoftUniBazar.Constants.CommonConstants;

    public class Category
    {
        [Key]
        [Comment("Category identifier")]
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength, ErrorMessage = DefaultErrorMessage)]
        [Comment("Category name")]
        public string Name { get; set; } = null!;

        public ICollection<Ad> Ads { get; set; } = new HashSet<Ad>();
    }
}
