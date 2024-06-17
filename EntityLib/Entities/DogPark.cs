using EntityLib.Entities.Identity;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities
{
    public class DogPark
    {

        [Key]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public int? AuthorId { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }

        public List<DogParkFacilityType>? Facilities { get; set; }

        public NpgsqlPolygon? Bounds { get; set; }
        public float? SquareKilometers { get; set; }

        [Required]
        public Place? Place { get; set; }
        public ApplicationUser? Author { get; set; }

        public DogPark()
        {
            DateAdded = DateTime.UtcNow;
        }
    }
}
