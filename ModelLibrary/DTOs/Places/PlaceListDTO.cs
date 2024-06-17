using FisSst.BlazorMaps;
using ModelLib.DTOs.AbstractDTOs;
using NpgsqlTypes;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Places
{
    public class PlaceListDTO : DTOWithLocation
    {
        override public int Id { get; set; }
        public string Name { get; set; }
        override public NpgsqlPoint Location { get; set; }
        public FacilityType Facility { get; set; }
        public float DistanceMeters { get; set; }
        public string? ProfileImgUrl { get; set; }
    }
}
