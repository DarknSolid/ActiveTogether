using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FisSst.BlazorMaps.Models.Basics
{
    public class LatLngBounds
    {
        public LatLngBounds()
        {
        }

        public LatLngBounds(LatLng southwest, LatLng northeast)
        {
            _southWest = southwest;
            _northEast = northeast;
        }

        public LatLng _southWest { get; set; }
        public LatLng _northEast { get; set; }

        public IEnumerable<LatLng> ToLatLng()
        {
            return new List<LatLng>() { _southWest, _northEast };
        }

        public double GetNorth()
        {
            return _northEast.Lat;
        }
        public double GetSouth()
        {
            return _southWest.Lat;
        }
        public double GetEast()
        {
            return _northEast.Lng;
        }
        public double GetWest()
        {
            return _southWest.Lng;
        }
    }
}
