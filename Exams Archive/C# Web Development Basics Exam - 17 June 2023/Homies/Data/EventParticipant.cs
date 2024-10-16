namespace Homies.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [PrimaryKey(nameof(HelperId), nameof(EventId))]
    public class EventParticipant
    {
        [Required]
        [Comment("Helper identifier")]
        public string HelperId { get; set; } = null!;

        [ForeignKey(nameof(HelperId))]  
        public IdentityUser Helper {  get; set; } = null!;

        [Required]
        [Comment("Event identifier")]
        public int EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; } = null!;
    }
}
