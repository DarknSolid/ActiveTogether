using ModelLib.DTOs.Dogs;
using static EntityLib.Entities.Enums;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.DTOs.Authentication
{
#nullable disable
    public class UserDetailedDTO : UserListDTO
    {
        public string Email { get; set; }
        public FriendShipStatus FriendShipStatus { get; set; }
        public bool HasCompany { get; set; }
        public int? CompanyId { get; set; }
        public FacilityType? CompanyType { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyProfilePictureUrl { get; set; }
        public IList<DogListDTO> Dogs { get; set; }
    }
}
