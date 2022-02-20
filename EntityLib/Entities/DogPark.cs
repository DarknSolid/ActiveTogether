using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class DogPark
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "geometry (point)")]
        public Point Location  { get; set; }

        public IEnumerable<DogParkFacility> Facilities { get; set; }
    }
}
