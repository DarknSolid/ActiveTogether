using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class Review
    {
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; } 

        [Required]
        [Range(0,5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public ApplicationUser User { get; set; }
        public Place Place { get; set; }

    }
}
