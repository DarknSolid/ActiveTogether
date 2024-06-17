using FisSst.BlazorMaps;

namespace ModelLib.DTOs
{
    public class SearchAreaDTO
    {
        public LatLng Center { get; set; }
        public int RadiusKilometers { get; set; }
    }
}
