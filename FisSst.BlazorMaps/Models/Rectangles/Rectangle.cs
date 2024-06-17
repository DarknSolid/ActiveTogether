using FisSst.BlazorMaps.JsInterops.Events;
using FisSst.BlazorMaps.Models.Basics;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FisSst.BlazorMaps.Models.Rectangles
{
    /// <summary>
    /// A rectangle
    /// </summary>
    public class Rectangle : Polyline
    {
        internal Rectangle(IJSObjectReference jsReference, IEventedJsInterop eventedJsInterop)
            : base(jsReference, eventedJsInterop)
        {
        }

        public async Task<Polyline> SetBounds(LatLngBounds bounds)
        {
            var latlngs = new List<LatLng>()
            {
                new LatLng {Lat = bounds.GetNorth(), Lng = bounds.GetWest()},
                new LatLng {Lat = bounds.GetNorth(), Lng = bounds.GetEast()},
                new LatLng {Lat = bounds.GetSouth(), Lng = bounds.GetEast()},
                new LatLng {Lat = bounds.GetSouth(), Lng = bounds.GetWest()},

            };
            return await this.SetLatLngs(latlngs);
        }
    }
}
