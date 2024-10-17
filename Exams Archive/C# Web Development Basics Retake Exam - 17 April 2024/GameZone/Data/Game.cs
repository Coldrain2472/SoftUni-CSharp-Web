namespace GameZone.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static GameZone.Constants.ModelConstants;

    public class Game
    {
        [Key]
        [Comment("Game Identifier")]
        public int Id {  get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        [Comment("Game title")]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        [Comment("Game description")]
        public string Description { get; set; } = null!;

        [Comment("The Url of the image")]
        public string? ImageUrl { get; set; }

        [Required]
        [Comment("Identifier of the game Publisher")]
        public string PublisherId { get; set; } = null!;

        [ForeignKey(nameof(PublisherId))]
        public IdentityUser Publisher { get; set; } = null!;

        [Required]
        [Comment("Release date")]
        public DateTime ReleasedOn { get; set; }

        [Required]
        [Comment("Game Genre")]
        public int GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]
        public Genre Genre { get; set; } = null!;

        public ICollection<GamerGame> GamersGames {  get; set; } = new HashSet<GamerGame>();

        [Comment("Shows whether the game is deleted or not")]
        public bool IsDeleted {  get; set; }
    }
}
