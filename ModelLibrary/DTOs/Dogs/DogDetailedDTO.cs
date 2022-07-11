
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Dogs
{
    public class DogDetailedDTO : DogListDTO
    {
        public string Description { get; set; }
        public DogWeightClass WeightClass{ get; set; }
    }
}
