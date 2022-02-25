using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using RazorLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorLib.AbstractClasses
{
    public abstract class MapSearcher
    {
        private float _previousNorth;
        private float _previousSouth;
        private float _previousWest;
        private float _previousEast;
        private readonly string _dogParkIconUrl = "https://developers.google.com/maps/documentation/javascript/examples/full/images/beachflag.png";//"https://www.clipartmax.com/png/full/436-4367818_iconathon-dog-park-icon-flat-circle-white-on-orange-circle.png";
        private List<Marker> _markers;
        private GoogleMap _map;
        private readonly int _minZoomLevel = 1;//11;

        protected MapSearcher()
        {
            _markers = new List<Marker>();
        }

        public void SetMap(GoogleMap map)
        {
            _map = map;
        }

        public async Task ToggleMarkers()
        {
            await DeleteAllMarkers();
        }

        private void EnsureMapSet()
        {
            if (_map == null)
            {
                throw new ArgumentNullException("Map is null. Please set it before calling this method");
            }
        }

        public async Task<MapMarkerSearchResult> UpdateMapIcons(MapFilterDTO filter)
        {
            EnsureMapSet();
            if (!await ShouldDisplayMapMarkers())
            {
                await DeleteAllMarkers();
                return null;
            }
            if (!(await ShouldFetchNewMapMarkers()))
            {
                return null;
            }
            await DeleteAllMarkers();
            await UpdatePreviousBounds();
            var searchRequest = new MapSearchDTO()
            {
                BoundsDTO = new BoundsDTO()
                {
                    North = _previousNorth,
                    South = _previousSouth,
                    East = _previousEast,
                    West = _previousWest,
                },
                MapFilterDTO = filter
            };
            var mapSearchResult = await FetchNewMapEntities(searchRequest);
            return await AddNewMapMarkers(mapSearchResult);
        }

        private async Task<bool> ShouldDisplayMapMarkers()
        {
            var curZoom = (int) await _map.InteropObject.GetZoom();
            return curZoom >= _minZoomLevel;
        }

        private async Task<MapMarkerSearchResult> AddNewMapMarkers(MapSearchResultDTO mapSearchResult)
        {
            MapMarkerSearchResult result = new MapMarkerSearchResult();
            var zoomLvl = await _map.InteropObject.GetZoom();
            foreach (DogParkListDTO dogPark in mapSearchResult.DogParkListDTOs)
            {
                dogPark.Name = "hejsa";
                var options = new MarkerOptions()
                {
                    Position = new LatLngLiteral() { Lat = dogPark.Latitude, Lng = dogPark.Longitude},
                    Map = _map.InteropObject,
                    Label = new MarkerLabel { Text = dogPark.Name, FontWeight = "bold",  },
                    Icon = new Icon()
                    {
                        Url = _dogParkIconUrl
                    }
                };

                var marker = await Marker.CreateAsync(_map.JsRuntime, options);
                _markers.Add(marker);
                result.DogParks.Add((marker, dogPark.Id));
            }
            return result;
        }

        abstract protected Task<MapSearchResultDTO> FetchNewMapEntities(MapSearchDTO mapSearchDTO);

        private async Task UpdatePreviousBounds()
        {
            var bounds = await _map.InteropObject.GetBounds();
            _previousNorth = (float)bounds.North;
            _previousSouth = (float)bounds.South;
            _previousWest = (float)bounds.West;
            _previousEast = (float)bounds.East;
        }

        private async Task DeleteAllMarkers()
        {
            await HideAllMarkers();
            _markers = new List<Marker>();
        }

        private async Task HideAllMarkers()
        {
            foreach (Marker marker in _markers)
            {
                await HideMarker(marker);
            }
        }

        private async Task HideMarker(Marker marker)
        {
            await marker.SetMap(null);
        }
        private async Task<bool> ShouldFetchNewMapMarkers()
        {
            // TODO add functionality to "cache" results
            return true;
        }

    }
}
