﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class DogPark
    {

        //TODO Images

        [Key]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        public Place Place { get; set; }

        public List<DogParkFacility> Facilities { get; set; }

    }
}
