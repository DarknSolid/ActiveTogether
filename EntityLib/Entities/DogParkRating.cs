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
        public float Rating { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        public DogPark DogPark { get; set; }
        public ApplicationUser User { get; set; }
    }
}
