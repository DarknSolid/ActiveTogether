using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class DogCheckIn
    {
        [Required]
        [ForeignKey(nameof(CheckIn))]
        public int CheckInId { get; set; }

        [Required]
        [ForeignKey(nameof(Dog))]
        public int DogId { get; set; }

        public Dog Dog { get; set; }
        public CheckIn CheckIn { get; set; }
    }
}
