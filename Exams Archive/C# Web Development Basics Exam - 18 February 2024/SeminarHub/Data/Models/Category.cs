namespace SeminarHub.Data.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using static SeminarHub.Constants.CommonConstants;

    public class Category
    {
        [Key]
        [Comment("Category identifier")]
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = DefaultErrorMessage)]
        [Comment("Category name")]
        public string Name { get; set; } = null!;

        public ICollection<Seminar> Seminars { get; set; } = new HashSet<Seminar>();
    }
}
