using Microsoft.AspNetCore.Components.Forms;
using ModelLib.ApiDTOs;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Instructors
{
    public class InstructorCreateDTO : LocationCreateDTO
    {
        public IEnumerable<InstructorCategory> Categories { get; set; }
        public IEnumerable<InstructorFacility> InstructorFacilities { get; set; }

        [AllowNull]
        [Url]
        public string? CompanyURL { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public FileDetailedDTO? CoverImage { get; set; }
        public FileDetailedDTO? ProfileImage { get; set; }

        public InstructorCreateDTO()
        {
            Categories = new List<InstructorCategory>();
            InstructorFacilities = new List<InstructorFacility>();
        }
    }
}
