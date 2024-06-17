using FisSst.BlazorMaps;
using FisSst.BlazorMaps.Models.Tooltips;
using Microsoft.AspNetCore.Components;
using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using ModelLib.DTOs.DogPark;
using RazorLib.Interfaces;

namespace RazorLib.Models.MapSearch
{
    public class DogParkMapSearcher : MapSearcherAbstract<DogParkListDTO>
    {
        public DogParkMapSearcher(IApiClient apiClient, IMarkerFactory markerFactory, IIconFactory iconFactory, NavigationManager navigationManager) 
            : base(apiClient, markerFactory, iconFactory, navigationManager)
        {
        }

        protected override MarkerOptions ConfigureMarkerOptions(DogParkListDTO dto, MarkerOptions markerOptions)
        {
            return markerOptions; 
        }

        protected override TooltipOptions ConfigureTooltipOptions(DogParkListDTO dto, TooltipOptions tooltipOptions)
        {
            return tooltipOptions;
        }

        protected override IconOptions ConfigureIconOptions(DogParkListDTO dto, IconOptions iconOptions)
        {
            // TODO set instructor icon
            return iconOptions;
        }

        protected override async Task<IEnumerable<DogParkListDTO>> FetchNewMapEntities(SearchAreaDTO searchArea)
        {
            return await _apiClient.GetDogParkListByArea(searchArea);
        }

        protected override Func<MouseEvent, Task> ConfigureOnMapMarkerClicked(DogParkListDTO dto)
        {
            return async (MouseEvent e) => _navigationManager.NavigateToDogParkDetails(dto.Id);
        }
    }
}
