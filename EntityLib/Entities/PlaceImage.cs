using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class PlaceImages
    {
        [Key]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        [Required]
        public string ImageKey { get; set; }

        public Place Place { get; set; }
    }
}
