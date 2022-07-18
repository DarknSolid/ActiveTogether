using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static EntityLib.Entities.Enums;

namespace MobileBlazor.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static void GoBack(this NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            jsRuntime.InvokeVoidAsync("history.back");
        }

        public static void NavigateToReviewCreate(this NavigationManager navigationManager, int id, FacilityType reviewType, string revieweeName, bool isReviewUpdate) 
        {
            navigationManager.NavigateTo(RoutingConstants.CREATE_REVIEW + $"{id}/{reviewType}/{revieweeName}/{isReviewUpdate}");
        }

        public static void NavigateToCreateDog(this NavigationManager navigationManager, int? dogId)
        {
            var url = RoutingConstants.CREATE_DOG;
            if (dogId.HasValue)
            {
                url += dogId.Value;
            }
            navigationManager.NavigateTo(url);
        }

        public static void NavigateToDogDetails(this NavigationManager navigationManager, int dogId)
        {
            navigationManager.NavigateTo(RoutingConstants.DOG_DETAILS + dogId);
        }

        public static void NavigateToDogParkDetails(this NavigationManager navigationManager, int dogParkId)
        {
            navigationManager.NavigateTo(RoutingConstants.DOG_PARK_DETAILS + dogParkId);
        }
    }
}
