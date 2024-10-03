namespace CinemaApp.Web.ViewModels.Movie
{
    using System.ComponentModel.DataAnnotations;
    using static Common.EntityValidationConstants.Movie;
    using static Common.EntityValidationMessages.Movie;

    public class AddMovieInputModel
    {
        [Required(ErrorMessage = TitleRequiredMessage)]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = GenreRequiredMessage)]
        [MinLength(GenreMinLength)]
        [MaxLength(GenreMaxLength)]
        public string Genre { get; set; } = null!;

        [Required(ErrorMessage = ReleaseDateRequiredMessage)]
        public string ReleaseDate { get; set; } = null!;

        [Required(ErrorMessage = DurationRequiredMessage)]
        [Range(DurationMinValue, DurationMaxValue)]
        public int Duration { get; set; }

        [Required(ErrorMessage = DirectorRequiredMessage)]
        [MinLength(DirectorNameMinLength)]
        [MaxLength(DirectorNameMaxLength)]
        public string Director { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLenght)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
    }
}
