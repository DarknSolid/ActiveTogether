using FisSst.BlazorMaps;
using FisSst.BlazorMaps.Models.Tooltips;
using Microsoft.AspNetCore.Components;
using ModelLib.DTOs;
using ModelLib.DTOs.Instructors;
using RazorLib.Interfaces;

namespace RazorLib.Models.MapSearch
{
    public class InstructorMapSearcher : MapSearcherAbstract<DogTrainerListDTO>
    {
        public InstructorMapSearcher(IApiClient apiClient, IMarkerFactory markerFactory, IIconFactory iconFactory, NavigationManager navigationManager) 
            : base(apiClient, markerFactory, iconFactory, navigationManager)
        {
        }

        protected override MarkerOptions ConfigureMarkerOptions(DogTrainerListDTO dto, MarkerOptions markerOptions)
        {
            return markerOptions; 
        }

        protected override TooltipOptions ConfigureTooltipOptions(DogTrainerListDTO dto, TooltipOptions tooltipOptions)
        {
            return tooltipOptions;
        }

        protected override IconOptions ConfigureIconOptions(DogTrainerListDTO dto, IconOptions iconOptions)
        {
            // TODO set instructor icon
            return iconOptions;
        }

        protected override async Task<IEnumerable<DogTrainerListDTO>> FetchNewMapEntities(SearchAreaDTO searchArea)
        {
            return await _apiClient.GetDogInstructorListByArea(searchArea);
        }

        protected override Func<MouseEvent, Task> ConfigureOnMapMarkerClicked(DogTrainerListDTO dto)
        {
            return async (MouseEvent e) => _navigationManager.NavigateToDogInstructor(dto.Id);
        }
    }
}
