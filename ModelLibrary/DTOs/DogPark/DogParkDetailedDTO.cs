using NpgsqlTypes;
using static EntityLib.Entities.Enums;
using static ModelLib.DTOEnums;

namespace ModelLib.DTOs.DogPark
{
    public class DogParkDetailedDTO : DogParkListDTO
    {
        public string Description { get; set; }
        public double Rating { get; set; }
        public IEnumerable<DogParkFacilityType> Facilities { get; set; }
        public List<string> ImageUrls { get; set; }
        public int TotalReviews { get; set; }

        public List<NpgsqlPoint>? Bounds { get; set; }
        public float? SquareKilometers { get; set; }

        public DogParkDetailedDTO()
        {
            ImageUrls = new List<string>();
            Facilities = new List<DogParkFacilityType>();
        }

        public ReviewStatus CurrentReviewStatus { get; set; }
    }
}
