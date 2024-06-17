using FisSst.BlazorMaps;
using FisSst.BlazorMaps.Models.Basics;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using Point = NetTopologySuite.Geometries.Point;

namespace ModelLib.Utils
{
    public static class Geographics
    {

        public enum DistanceUnit
        {
            Kilometers,
            Meters,
            Miles
        }

        /// <summary>
        /// Given a point and a radius in meters, returns the bounding box as a square.
        /// Note that the square's diagonals will make the radius greater than the desired radius
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radiusInMeters"></param>
        /// <returns></returns>
        public static LatLngBounds SphereToBounds(LatLng center, int radiusInMeters)
        {
            // documentation: https://gis.stackexchange.com/questions/2951/algorithm-for-offsetting-a-latitude-longitude-by-some-amount-of-meters
            var earthsRadius = 6378137d;
            var pi180 = 180d / Math.PI;

            var dLat = radiusInMeters / earthsRadius * pi180;
            var dLon = radiusInMeters / (earthsRadius * Math.Cos(center.Lat * Math.PI / 180d)); // yes, it is meant to be center.Lat.
            dLon = dLon * pi180;

            return new LatLngBounds
            {
                _southWest = new LatLng(lat: center.Lat - dLat, lng: center.Lng - dLon),
                _northEast = new LatLng(lat: center.Lat + dLat, lng: center.Lng + dLon)
            };
        }

        public static double DistanceTo(this Point from, Point to, DistanceUnit unit = DistanceUnit.Meters)
        {
            var fromNpg = new NpgsqlPoint(x: from.X, y: from.Y);
            var toNpg = new NpgsqlPoint(x: to.X, y: to.Y);
            return fromNpg.DistanceTo(toNpg, unit);
        }

        /// <summary>
        /// Calculates the distance to a coordinate in the specified distance unit. 
        /// This method is platform agnostic as compared to the .NET Framework's GeoCoordinate library
        /// code taken from https://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates
        /// </summary>
        /// <returns></returns>
        public static double DistanceTo(this NpgsqlPoint from, NpgsqlPoint to, DistanceUnit unit = DistanceUnit.Meters)
        {
            double rlat1 = Math.PI * from.Y / 180;
            double rlat2 = Math.PI * to.Y / 180;
            double theta = from.X - to.X;
            double rtheta = Math.PI * theta / 180;
            double distMiles =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            distMiles = Math.Acos(distMiles);
            distMiles = distMiles * 180 / Math.PI;
            distMiles = distMiles * 60 * 1.1515;
            switch (unit)
            {
                case DistanceUnit.Kilometers:
                    return distMiles * 1.609344;
                case DistanceUnit.Meters:
                    return distMiles * 1609.344;
                case DistanceUnit.Miles:
                    return distMiles;
            }

            return distMiles;
        }
    }
}
