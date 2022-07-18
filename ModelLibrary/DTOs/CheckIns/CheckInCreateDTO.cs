
namespace ModelLib.DTOs.CheckIns
{
    public class CheckInCreateDTO
    {
        public int PlaceId { get; set; }
        public List<int> DogsToCheckIn { get; set; }
    }
}
