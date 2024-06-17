using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class Company
    {
        [Key]
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public int ApplicationUserId { get; set; }

        [Url]
        public string? CompanyURL { get; set; }

        [Required, StringLength(15)]
        public string Phone { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; }
        public string? CoverImageBlobUrl { get; set; }

        public DateTime CreatedAt { get; set; }


        public Place Place { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<InstructorCompanyCategory> InstructorCategories { get; set; }
        public ICollection<InstructorCompanyFacility> InstructorFacilities { get; set; }
    }
}
