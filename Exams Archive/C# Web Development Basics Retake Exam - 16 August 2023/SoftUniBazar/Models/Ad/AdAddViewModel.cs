namespace SoftUniBazar.Models.Ad
{
    using SoftUniBazar.Models.Category;
    using System.ComponentModel.DataAnnotations;
    using static SoftUniBazar.Constants.CommonConstants;

    public class AdAddViewModel
    {
        [Required]
        [StringLength(AdNameMaxLength, MinimumLength = AdNameMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(AdDescriptionMaxLength, MinimumLength = AdDescriptionMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Description { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public ICollection<CategoryViewModel> Categories { get; set; } = new HashSet<CategoryViewModel>();
    }
}
