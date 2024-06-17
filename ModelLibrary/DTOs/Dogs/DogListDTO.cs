
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Dogs
{
    public class DogListDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Birth { get; set; }

        public bool IsGenderMale { get; set; }

        public DogRace Race { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
