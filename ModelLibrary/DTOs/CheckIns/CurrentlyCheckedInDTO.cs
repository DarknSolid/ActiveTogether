using ModelLib.DTOs.Dogs;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.CheckIns
{
    public class CurrentlyCheckedInDTO
    {
        public List<DogListDTO> Dogs { get; set; }
        public int PlaceId { get; set; }
        public FacilityType FacilityType { get; set; }
        public DateTime CheckInDate { get; set; }
        public CheckInMood Mood { get; set; }
    }
}
