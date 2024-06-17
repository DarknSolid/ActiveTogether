using EntityLib.Entities.AbstractClasses;
using System.ComponentModel.DataAnnotations;

namespace ModelLib.DTOs.Reviews
{
    public class ReviewCreateDTO : IntegerId
    {
        override public int Id { get; set; }
        public int Rating { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Length can't be more than 50 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "Length can't be more than 2000 characters.")]
        public string Description { get; set; }
    }
}
