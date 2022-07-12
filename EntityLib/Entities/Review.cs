using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities
{
    public class Review
    {
        [Column(nameof(ReviewerId))]
        [ForeignKey(nameof(ApplicationUser))]
        public int ReviewerId { get; set; }
        public int RevieweeId { get; set; } 
        public FacilityType ReviewType { get; set; }

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
        public ApplicationUser Reviewer { get; set; }

    }
}
