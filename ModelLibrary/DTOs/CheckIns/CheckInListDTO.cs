using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.Dogs;

namespace ModelLib.DTOs.CheckIns
{
    public class CheckInListDTO
    {
        public List<DogListDTO> Dogs { get; set; }
        public UserListDTO User { get; set; }
        public DateTime CheckedIn { get; set; }
        public DateTime? CheckedOut { get; set; }
    }
}
