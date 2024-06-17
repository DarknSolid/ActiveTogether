using FisSst.BlazorMaps;
using FisSst.BlazorMaps.Models.Basics;
using FisSst.BlazorMaps.Models.Tooltips;
using Microsoft.AspNetCore.Components;
using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.Places;
using RazorLib.Interfaces;
using RazorLib.Utils;

namespace RazorLib.Models.MapSearch
{
    public abstract class MapSearcherAbstract<ListDTO>
        where ListDTO : PlaceListDTO
    {
        private readonly string _defaultPinIcon = "https://developers.google.com/maps/documentation/javascript/examples/full/images/beachflag.png";//"https://www.clipartmax.com/png/full/436-4367818_iconathon-dog-park-icon-flat-circle-white-on-orange-circle.png";
        private List<Marker> _markers;
        private Map _map;
        private readonly IMarkerFactory _markerFactory;
        private readonly IIconFactory _iconFactory;

        protected readonly IApiClient _apiClient;
        public NavigationManager _navigationManager { get; set; }

        protected MapSearcherAbstract(IApiClient apiClient, IMarkerFactory markerFactory, IIconFactory iconFactory, NavigationManager navigationManager)
        {
            _markers = new List<Marker>();
            _apiClient = apiClient;
            _markerFactory = markerFactory;
            _iconFactory = iconFactory;
            _navigationManager = navigationManager;
        }

        public async Task RemoveAllMarkers()
        {
            if (_map == null)
            {
                return;
            }
            foreach (Marker marker in _markers)
            {
                await marker.RemoveFrom(_map);
            }
            _markers = new List<Marker>();
        }

        public async Task UpdateMapIcons(Map map, SearchAreaDTO searchArea)
        {
            _map = map;
            await RemoveAllMarkers();

            var mapSearchResult = await FetchNewMapEntities(searchArea);
            await AddNewMapMarkers(mapSearchResult);
        }

        private async Task AddNewMapMarkers(IEnumerable<ListDTO> dtos)
        {
            foreach (ListDTO dto in dtos)
            {
                var center = new LatLng()
                {
                    Lat = dto.Location.Y,
                    Lng = dto.Location.X
                };

                int scale = 2;

                #region logo
                var iconOptions = new IconOptions
                {
                    IconUrl = dto.ProfileImgUrl ?? _defaultPinIcon,
                    IconSize = new Point(x: 30 * scale, y: 30 * scale),
                    IconAnchor = new(15 * scale, (15 + 39) * scale),
                };

                var pinIcon = await _iconFactory.Create(ConfigureIconOptions(dto, iconOptions));

                var markerOptions = new MarkerOptions()
                {
                    Title = dto.Name,
                    IconRef = pinIcon,
                };

                markerOptions = ConfigureMarkerOptions(dto, markerOptions);

                var marker = await _markerFactory.CreateAndAddToMap(center, _map, markerOptions);

                var tooltipOptions = new TooltipOptions
                {
                    Permanent = true,
                    Direction = "bottom",
                    Opacity = 1,
                    ClassName = "leaflet-tooltip-custom-text-only",
                    Offset = new Point(0, 15)
                };
                tooltipOptions = ConfigureTooltipOptions(dto, tooltipOptions);

                await marker.BindTooltip(dto.Name,
                    tooltipOptions);

                _markers.Add(marker);
                #endregion

                #region basePin
                var pinIconOptions = new IconOptions
                {
                    IconUrl = @Images.IconPinLocationImageFrame,
                    IconSize = new Point(47*scale,61*scale),
                    IconAnchor = new(23*scale, 60*scale),
                };

                pinIcon = await _iconFactory.Create(pinIconOptions);

                var pinMarkerOptions = new MarkerOptions()
                {
                    Title = dto.Name,
                    IconRef = pinIcon,
                    
                };

                marker = await _markerFactory.CreateAndAddToMap(center, _map, pinMarkerOptions);
                await marker.OnClick(ConfigureOnMapMarkerClicked(dto));
                _markers.Add(marker);
                #endregion
            }
        }

        protected abstract Func<MouseEvent, Task> ConfigureOnMapMarkerClicked(ListDTO dto);

        protected abstract IconOptions ConfigureIconOptions(ListDTO dto, IconOptions iconOptions);

        /// <summary>
        /// Modifies the existing marker options, which has the name and icon set per default.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="markerOptions">the marker options to modify</param>
        /// <returns>the modified marker options</returns>
        protected abstract MarkerOptions ConfigureMarkerOptions(ListDTO dto, MarkerOptions markerOptions);

        /// <summary>
        /// Modifies the existing tooltip options, which has the name and icon set per default.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="tooltipOptions">the tooltip options to modify</param>
        /// <returns>the modified tooltip options</returns>
        protected abstract TooltipOptions ConfigureTooltipOptions(ListDTO dto, TooltipOptions tooltipOptions);

        protected abstract Task<IEnumerable<ListDTO>> FetchNewMapEntities(SearchAreaDTO searchArea);
    }
}
