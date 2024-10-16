namespace Homies.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using static Homies.Constants.CommonConstants;

    public class Type
    {
        [Key]
        [Comment("Type identifier")]
        public int Id { get; set; }

        [Required]
        [StringLength(TypeNameMaxLength)]
        [Comment("Type name")]
        public string Name { get; set; } = null!;

        public ICollection<Event> Events { get; set; } = new HashSet<Event>();
    }
}
