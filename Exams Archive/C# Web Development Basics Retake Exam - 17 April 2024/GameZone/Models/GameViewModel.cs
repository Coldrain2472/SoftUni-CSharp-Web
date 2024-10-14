namespace GameZone.Models
{
    using GameZone.Data;
    using System.ComponentModel.DataAnnotations;
    using static GameZone.Constants.ModelConstants;

    public class GameViewModel
    {
        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
        public string Title { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        public string ReleasedOn { get; set; } = DateTime.Today.ToString(GameReleasedOnDateFormat);

        [Required]
        public int GenreId { get; set; }

        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
