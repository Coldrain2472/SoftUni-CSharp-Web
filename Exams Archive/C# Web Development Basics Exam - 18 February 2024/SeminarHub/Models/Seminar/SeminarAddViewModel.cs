namespace SeminarHub.Models.Seminar
{
    using System.ComponentModel.DataAnnotations;
    using SeminarHub.Models.Category;
    using static SeminarHub.Constants.CommonConstants;

    public class SeminarAddViewModel
    {
        [StringLength(TopicMaxLength, MinimumLength = TopicMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Topic { get; set; } = null!;

        [StringLength(LecturerMaxLength, MinimumLength = LecturerMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Lecturer { get; set; } = null!;

        [StringLength(DetailsMaxLength, MinimumLength = DetailsMinLength, ErrorMessage = DefaultErrorMessage)]
        public string Details { get; set; } = null!;

        public string DateAndTime { get; set; } = null!;

        [Range(DurationMinValue, DurationMaxValue)]
        public int Duration { get; set; }

        public int CategoryId { get; set; }

        public ICollection<CategoryViewModel> Categories { get; set; } = new HashSet<CategoryViewModel>();
    }
}
