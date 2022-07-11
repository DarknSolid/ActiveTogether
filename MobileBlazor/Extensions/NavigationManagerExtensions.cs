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

        public static void NavigateToReviewCreate(this NavigationManager navigationManager, int id, ReviewType reviewType, string revieweeName, bool isReviewUpdate) 
        {
            navigationManager.NavigateTo(RoutingConstants.CREATE_REVIEW + $"{id}/{reviewType}/{revieweeName}/{isReviewUpdate}");
        }
    }
}
