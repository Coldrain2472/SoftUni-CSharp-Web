namespace SeminarHub.Models
{
    using System.ComponentModel.DataAnnotations;
    using static SeminarHub.Constants.CommonConstants;

    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
