using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using EntityLib.Entities.Constants;
using Microsoft.EntityFrameworkCore;
using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.PostsAndComments;

namespace EntityLib.Entities
{
    [Index(nameof(Name))]
    public class Place : EntityWithLocation
    {
        [Required]
        [Key]
        public override int Id { get; set; }
        [Required]
        [Column(TypeName = "geometry (point)")]
        public override Point Location { get; set; }
        [Required]
        public Enums.FacilityType FacilityType { get; set; }

        [Required]
        [MaxLength(PlaceConstants.NameMaxLength)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(PlaceConstants.DescriptionMaxLength)]
        public string? Description { get; set; }
        public string? ProfileImageBlobUrl { get; set; }

        public ICollection<CheckIn>? CheckIns { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public Company Company { get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}
