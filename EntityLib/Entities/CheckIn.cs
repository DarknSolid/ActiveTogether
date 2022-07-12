using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace EntityLib.Entities
{
    public class CheckIn
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        [Required]
        public int FacilityId { get; set; }

        [Required]
        public Enums.FacilityType FacilityType { get; set; }

        public ICollection<DogCheckIn> DogCheckIns { get; set; }
    }
}
