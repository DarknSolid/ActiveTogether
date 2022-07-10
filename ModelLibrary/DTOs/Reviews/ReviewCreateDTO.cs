using EntityLib.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Reviews
{
    public class ReviewCreateDTO
    {
        public int RevieweeId { get; set; }
        public ReviewType ReviewType { get; set; }
        public int Rating { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Length can't be more than 50 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "Length can't be more than 2000 characters.")]
        public string Description { get; set; }
    }
}
