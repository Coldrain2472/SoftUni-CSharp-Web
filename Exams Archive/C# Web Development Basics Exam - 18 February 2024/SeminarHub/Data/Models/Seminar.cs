namespace SeminarHub.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static SeminarHub.Constants.CommonConstants;

    public class Seminar
    {
        [Key]
        [Comment("Seminar identifier")]
        public int Id { get; set; }

        [Required]
        [StringLength(TopicMaxLength, MinimumLength = TopicMinLength, ErrorMessage = DefaultErrorMessage)]
        [Comment("Topic of the seminar")]
        public string Topic { get; set; } = null!;

        [StringLength(LecturerMaxLength, MinimumLength = LecturerMinLength, ErrorMessage = DefaultErrorMessage)]
        [Comment("Seminar's lecturer")]
        public string Lecturer { get; set; } = null!;

        [StringLength(DetailsMaxLength, MinimumLength = DetailsMinLength, ErrorMessage = DefaultErrorMessage)]
        [Comment("Seminar details")]
        public string Details { get; set; } = null!;

        [Required]
        [Comment("Organizer identifier")]
        public string OrganizerId { get; set; } = null!;

        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        [Comment("Seminar's date and time")]
        public DateTime DateAndTime { get; set; }

        [Range(DurationMinValue, DurationMaxValue, ErrorMessage = DefaultErrorMessage)]
        [Comment("Seminar's duration")]
        public int Duration { get; set; }

        [Required]
        [Comment("Category identifier")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public ICollection<SeminarParticipant> SeminarsParticipants { get; set; } = new HashSet<SeminarParticipant>();
    }
}
