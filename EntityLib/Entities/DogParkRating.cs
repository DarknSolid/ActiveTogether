using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLib.Entities
{
    public class DogParkRating
    {
        public int UserId { get; set; }
        public int DogParkId { get; set; } 

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
        public DogPark DogPark { get; set; }
        public ApplicationUser User { get; set; }
    }
}
