namespace SeminarHub.Models.Category
{
    using System.ComponentModel.DataAnnotations;
    using static SeminarHub.Constants.CommonConstants;

    public class CategoryViewModel
    {
        public int Id { get; set; }

        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Name { get; set; } = null!;
    }
}
