using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.CheckIns
{
    public class CurrentlyCheckedInDTO
    {
        public int PlaceId { get; set; }
        public PlaceType FacilityType { get; set; }
        public DateTime CheckInDate { get; set; }
        public CheckInMood Mood { get; set; }
    }
}
