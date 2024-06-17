
using EntityLib.Entities;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.CheckIns
{
    public class CheckInCreateDTO
    {
        public int PlaceId { get; set; }
        public CheckInMood Mood { get; set; }
    }
}
