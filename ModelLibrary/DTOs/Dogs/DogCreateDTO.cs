using ModelLib.ApiDTOs;
using System.ComponentModel.DataAnnotations;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Dogs
{
    public class DogCreateDTO
    {
        [Required]
        [StringLength(25, ErrorMessage = "Max 25 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage ="Max 2000 chararcters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select date of birth")]
        public DateTime Birth { get; set; }
        
        [Required(ErrorMessage ="Please select gender")]
        public bool IsGenderMale { get; set; }

        [Required(ErrorMessage = "Please select your dog's race")]
        public DogRace Race { get; set; }

        [Required]
        public DogWeightClass WeightClass { get; set; }

        public FileDetailedDTO? ProfilePicture { get; set; }

    }
}
