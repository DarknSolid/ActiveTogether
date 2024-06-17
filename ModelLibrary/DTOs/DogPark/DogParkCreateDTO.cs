using EntityLib.Entities.Constants;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.DogPark
{
    public class DogParkCreateDTO : LocationCreateDTO
    {
        public IList<NpgsqlPoint>? Bounds { get; set; }
        public float? SquareKilometers { get; set; }

        public IEnumerable<DogParkFacilityType> Facilities { get; set; }

        public DogParkCreateDTO()
        {
            Point = new();
            Facilities = new List<DogParkFacilityType>();
        }
    }
}
