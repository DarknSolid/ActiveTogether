using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.EventsAndMeetups.Enums;

namespace EntityLib.Entities.EventsAndMeetups
{
    public class GatheringParticipant
    {
        [Required]
        [ForeignKey(nameof(Meetup))]
        public int MeetupId { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [Required]
        public GatheringParticipationState MyProperty { get; set; }

        public Gathering? Meetup { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
