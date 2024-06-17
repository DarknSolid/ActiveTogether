using ModelLib.DTOs.Authentication;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.CheckIns
{
    public class CheckInListDTO
    {
        public int Id { get; set; }
        public UserListDTO User { get; set; }
        public DateTime CheckedInDate { get; set; }
        public DateTime? CheckedOutDate { get; set; }
        public CheckInMood Mood { get; set; }
    }
}
