using EntityLib.Entities.Constants;
using EntityLib.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities
{
    public class PendingDogPark
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int RequesterId { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        public PlaceType FacilityType { get; set; }

        [Required]
        [Column(TypeName = "geometry (point)")]
        public Point? Location { get; set; }

        [Required]
        [MaxLength(PlaceConstants.NameMaxLength)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(PlaceConstants.DescriptionMaxLength)]
        public string? Description { get; set; }

        public List<DogParkFacilityType>? Facilities { get; set; }

        public NpgsqlPolygon? Bounds { get; set; }
        public float? SquareKilometers { get; set; }

        public ApplicationUser? Requester { get; set; }

        public PendingDogPark()
        {
            RequestDate = DateTime.UtcNow;
        }

    }
}
