using EntityLib.Entities.EventsAndMeetups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities.Gatherings
{
    public class Event
    {
        [Key]
        [ForeignKey(nameof(Gathering))]
        public int GatheringId { get; set; }

        [Required]
        [ForeignKey(nameof(Company))]
        public int CompanyId { get; set; }


        public Gathering? Gathering { get; set; }
        public Company? Company { get; set; }
    }
}
