using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLib.Entities
{
    public class DogCheckIn
    {
        [Required]
        [Key]
        [ForeignKey(nameof(CheckIn))]
        public int CheckInId { get; set; }

        [Required]
        [ForeignKey(nameof(Dog))]
        public int DogId { get; set; }

        public Dog Dog { get; set; }
        public CheckIn CheckIn { get; set; }
    }
}
