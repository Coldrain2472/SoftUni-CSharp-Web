namespace DeskMarket.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DeskMarket.Constants.CommonConstants;

    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength)]
        public string Name { get; set; } = null!;
    }
}
