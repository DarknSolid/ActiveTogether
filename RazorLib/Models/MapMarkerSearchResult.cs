using GoogleMapsComponents.Maps;

namespace RazorLib.Models
{
    public  class MapMarkerSearchResult
    {
        public List<(Marker marker, int id)> DogParks { get; set; }

        public MapMarkerSearchResult()
        {
            DogParks = new List<(Marker marker, int id)>();
        }
    }
}
