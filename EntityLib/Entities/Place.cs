using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace EntityLib.Entities
{
    public class Place
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Enums.FacilityType FacilityType { get; set; }

        [Required]
        [Column(TypeName = "geometry (point)")]
        public Point Location { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public ICollection<CheckIn> CheckIns { get; set; }
        public ICollection<Review> Reviews { get; set; }

    }
}
