using static EntityLib.Entities.Enums;
using static ModelLib.DTOEnums;

namespace ModelLib.DTOs.Instructors
{
    public class InstructorDetailedDTO : DogTrainerListDTO
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public IEnumerable<InstructorCategory> Categories { get; set; }
        public IEnumerable<InstructorCategory> ActiveCategories { get; set; }
        public IEnumerable<InstructorFacility> Facilities { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string? OwnerProfilePictureUrl { get; set; }

        public string? CompanyURL { get; set; }

        public ReviewStatus CurrentReviewStatus { get; set; }
    }
}
