
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Dogs
{
    public class DogDetailedDTO : DogListDTO
    {
        public string Description { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string? UserProfilePictureUrl { get; set; }
        public DogWeightClass WeightClass{ get; set; }
    }
}
