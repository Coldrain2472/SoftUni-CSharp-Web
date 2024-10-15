namespace SeminarHub.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static SeminarHub.Constants.CommonConstants;

    public class Seminar
    {
        [Key]
        [Comment("Unique Identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(TopicMaxLength)]
        [Comment("Topic of the seminar")]
        public string Topic { get; set; } = null!;

        [Required]
        [MaxLength(LecturerMaxLength)]
        [Comment("Seminar's lecturer")]
        public string Lecturer { get; set; } = null!;

        [Required]
        [MaxLength(DetailsMaxLength)]
        [Comment("Seminar details")]
        public string Details { get; set; } = null!;

        [Required]
        [Comment("Identifier of the organizer")]
        public string OrganizerId { get; set; } = null!;

        [ForeignKey(nameof(OrganizerId))]
        public IdentityUser Organizer { get; set; } = null!;

        [Comment("Date")]
        public DateTime DateAndTime { get; set; }

        [Range(DurationMinValue, DurationMaxValue)]
        [Comment("Seminar duration")]
        public int Duration { get; set; }

        [Required]
        [Comment("Category identifier")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public ICollection<SeminarParticipant> SeminarsParticipants {  get; set; } = new HashSet<SeminarParticipant>();
    }
}
