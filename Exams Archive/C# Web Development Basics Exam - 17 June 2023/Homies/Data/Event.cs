namespace Homies.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Homies.Constants.CommonConstants;

    public class Event
    {
        [Key]
        [Comment("Event identifier")]
        public int Id { get; set; }

        [Required]
        [StringLength(EventNameMaxLength)]
        [Comment("Event name")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength)]
        [Comment("Event description")]
        public string Description { get; set; } = null!;

        [Required]
        public string OrganiserId { get; set; } = null!;

        [ForeignKey(nameof(OrganiserId))]
        public IdentityUser Organiser { get; set; } = null!;

        [Required]
        [Comment("Date of the event creation")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [Comment("Event start date")]
        public DateTime Start { get; set; }

        [Required]
        [Comment("Event end date")]
        public DateTime End { get; set; }

        [Required]
        public int TypeId { get; set; }

        [ForeignKey(nameof(TypeId))]
        public Type Type {  get; set; } = null!;

        public bool IsDeleted { get; set; }

        public ICollection<EventParticipant> EventsParticipants { get; set; } = new HashSet<EventParticipant>();    
    }
}
