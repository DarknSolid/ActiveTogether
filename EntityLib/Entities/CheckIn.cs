using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class CheckIn
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        [Required]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        [Required]
        public Enums.CheckInMood Mood { get; set; }

        public ApplicationUser User { get; set; }
        public Place Place { get; set; }
        public ICollection<DogCheckIn> DogCheckIns { get; set; }


    }
}
