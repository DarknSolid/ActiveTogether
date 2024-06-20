using EntityLib.Entities.Gatherings;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities.EventsAndMeetups
{
    public class Gathering
    {
        [Key]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        public Place? Place { get; set; }

        public DateTime DateTime { get; set; }

        [ForeignKey(nameof(Place))]
        public int? HeldAtPlaceId { get; set; }
        public Place? HeldAtPlace { get; set; }
        public ICollection<GatheringParticipant> Participants { get; set; }
        public ICollection<Event> Events{ get; set; }

    }
}
