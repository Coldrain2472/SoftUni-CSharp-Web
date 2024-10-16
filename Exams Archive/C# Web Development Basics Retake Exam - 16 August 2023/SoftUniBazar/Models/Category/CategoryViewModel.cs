namespace SoftUniBazar.Models.Category
{
    using System.ComponentModel.DataAnnotations;
    using static SoftUniBazar.Constants.CommonConstants;

    public class CategoryViewModel
    {
        public int Id { get; set; }

        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Name { get; set; } = null!;
    }
}
