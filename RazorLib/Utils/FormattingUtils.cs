
using FisSst.BlazorMaps;

namespace RazorLib.Utils
{
    public static class FormattingUtils
    {
        public static string FormatDistance(float distanceMeters)
        {
            return (distanceMeters/1000).ToString("n1") + " km";
        }

        public static string FormatLocation(LatLng location)
        {
            return $"{(float)location.Lat}, {(float)location.Lng}";
        }
    }
}
