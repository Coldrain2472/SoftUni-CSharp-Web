namespace DeskMarket.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DeskMarket.Constants.CommonConstants;

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Name { get; set; } = null!;

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
