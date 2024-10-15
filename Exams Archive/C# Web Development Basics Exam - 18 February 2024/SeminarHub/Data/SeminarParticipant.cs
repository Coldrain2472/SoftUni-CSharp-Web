namespace SeminarHub.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;

    [PrimaryKey(nameof(SeminarId), nameof(ParticipantId))]
    public class SeminarParticipant
    {
        [Comment("Identifier of the seminar")]
        public int SeminarId { get; set; }

        [ForeignKey(nameof(SeminarId))]
        public Seminar Seminar { get; set; } = null!;

        [Comment("Identifier of the participant")]
        public string ParticipantId { get; set; } = null!;

        [ForeignKey(nameof(ParticipantId))]
        public IdentityUser Participant {  get; set; } = null!;
    }
}
