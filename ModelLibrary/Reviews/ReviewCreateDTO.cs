using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Reviews
{
    public class ReviewCreateDTO
    {
        public int DogParkId { get; set; }
        public int CreatorId { get; set; }
        public int Rating { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "Length can't be more than 50 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(2000, ErrorMessage = "Length can't be more than 2000 characters.")]
        public string Comment { get; set; }
    }
}
