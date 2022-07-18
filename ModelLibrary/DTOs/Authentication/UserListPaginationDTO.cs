using ModelLib.ApiDTOs;

namespace ModelLib.DTOs.Authentication
{
    public class UserListPaginationDTO
    {
        public List<UserListDTO> Friends { get; set; }
        public PaginationResult PaginationResult { get; set; }
    }
}
