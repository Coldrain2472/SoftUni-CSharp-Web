namespace GameZone.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using static GameZone.Constants.ModelConstants;

    public class Genre
    {
        [Key]
        [Comment("Genre Identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Comment("Genre name")]
        public string Name { get; set; } = null!;

        public ICollection<Game> Games { get; set; } = new HashSet<Game>();
    }
}
