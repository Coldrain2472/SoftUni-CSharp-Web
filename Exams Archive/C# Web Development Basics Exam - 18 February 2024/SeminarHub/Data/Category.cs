namespace SeminarHub.Data
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
        [MaxLength(NameMaxLength)]
        [Comment("Category's name")]
        public string Name { get; set; } = null!;

        public ICollection<Seminar> Seminars { get; set; } = new HashSet<Seminar>();
    }
}
