using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ModelLib.DTOs.Posts;
using RazorLib.Utils;
using static EntityLib.Entities.Enums;

namespace RazorLib
{
    public static class NavigationManagerExtensions
    {

        public static void GoBack(this NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            jsRuntime.InvokeVoidAsync("history.back");
        }

        public static void NavigateToLogin(this NavigationManager navigationManager)
        {
            var currentRelativeUri = navigationManager.Uri.Replace(navigationManager.BaseUri, string.Empty);

            // on some pages, the api is called in parallel causing this function to be called in parallel, therefore we return after the first call:
            if (currentRelativeUri.StartsWith(RoutingConstants.LOGIN))
            {
                return;
            }
            navigationManager.NavigateTo(RoutingConstants.LOGIN + $"?callback={currentRelativeUri}");
        }

        public static void NavigateToLogin(this NavigationManager navigationManager, string callback)
        {
            navigationManager.NavigateTo(RoutingConstants.LOGIN + $"?callback={callback}");
        }

        public static void NavigateToRegisteredUserWelcome(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.NEW_USER_WELCOME);
        }

        public static void NavigateToHome(this NavigationManager navigationManager)
        {
            navigationManager.NavigateToDiscover(FacilityType.DogInstructor);
        }

        public static void NavigateToRegister(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.REGISTER);
        }

        public static void NavigateToForgotPassword(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.FORGOT_PASSWORD);
        }

        public static void NavigateToReviews(this NavigationManager navigationManager, int placeId)
        {
            navigationManager.NavigateTo(RoutingConstants.REVIEWS + $"{placeId}");
        }

        public static void NavigateToAbout(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.ABOUT);
        }

        public static void NavigateToCredits(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.CREDITS);
        }

        public static void NavigateToReviewCreate(this NavigationManager navigationManager, int id, string revieweeName) 
        {
            navigationManager.NavigateTo(RoutingConstants.CREATE_REVIEW + $"{id}/{revieweeName}/{false}");
        }

        public static void NavigateToReviewUpdate(this NavigationManager navigationManager, int placeId, int userId, string revieweeName)
        {
            navigationManager.NavigateTo(RoutingConstants.UPDATE_REVIEW + $"{placeId}/{userId}/{revieweeName}/{true}");
        }

        public static void NavigateToDiscover(this NavigationManager navigationManager, FacilityType facilityType)
        {
            navigationManager.NavigateTo(RoutingConstants.DISCOVER + facilityType.ToString());
        }


        public static void NavigateToFeed(this NavigationManager navigationManager, PostFilter? postFilter = null)
        {
            var queryParameters = new List<string>();
            if (postFilter != null)
            {
                queryParameters.Add(postFilter.Category.HasValue ? "category=" + postFilter.Category.ToString() : null);
                queryParameters.Add(postFilter.Area.HasValue ? "area=" + postFilter.Area.ToString() : null);
            }

            string queryParametersString = "";
            if (queryParameters.Any(s => !String.IsNullOrEmpty(s)))
            {
                queryParametersString = string.Join("&", queryParameters.Where(s => !string.IsNullOrEmpty(s)));
            }

            navigationManager.NavigateTo(RoutingConstants.FEED + "?" + queryParametersString);
        }

        public static void NavigateToPlace(this NavigationManager navigationManager, int placeId, FacilityType facilityType)
        {
            switch (facilityType)
            {
                case FacilityType.DogInstructor: NavigateToDogInstructor(navigationManager, placeId); break;
                case FacilityType.DogPark: NavigateToDogParkDetails(navigationManager, placeId); break;
                default: throw new NotImplementedException($"{facilityType} has not been defined in the navigation switch clause");
            }
        }

        public static void NavigateToDogParkDetails(this NavigationManager navigationManager, int dogParkId)
        {
            navigationManager.NavigateTo(RoutingConstants.DOG_PARK_DETAILS + dogParkId);
        }

        public static void NavigateToUser(this NavigationManager navigationManager, int userId)
        {
            navigationManager.NavigateTo(RoutingConstants.USER + userId);
        }
        public static void NavigateToDogInstructor(this NavigationManager navigationManager, int instructorId)
        {
            navigationManager.NavigateTo(RoutingConstants.INSTRUCTOR + instructorId);
        }

        public static void NavigateToUpdateDogInstructor(this NavigationManager navigationManager, int instructorId)
        {
            navigationManager.NavigateTo(RoutingConstants.UPDATE_INSTRUCTOR + instructorId);
        }

        public static void NavigateToDogTrainingDetails(this NavigationManager navigationManager, int id)
        {
            navigationManager.NavigateTo(RoutingConstants.DOG_TRAINING + id);
        }

        public static void NavigateToCreateDogTraining(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.CREATE_DOG_TRAINING);
        }

        public static void NavigateToUpdateDogTraining(this NavigationManager navigationManager, int id)
        {
            navigationManager.NavigateTo(RoutingConstants.UPDATE_DOG_TRAINING + id);
        }

        public static async Task OpenNewTab(this NavigationManager navigationManager, string url, IJSRuntime jSRuntime)
        {
            jSRuntime.InvokeAsync<object>("open", url, "_blank");
        }

        public static void NavigateToSettings(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.USER_SETTINGS);
        }

        public static void NavigateToCreateInstructor(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.CREATE_INSTRUCTOR);
        }

        public static void NavigateToEnterpriseSettings(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.ENTERPRISE_SETTINGS);
        }

        public static void NavigateToEnterpriseProfile(this NavigationManager navigationManager)
        {
            navigationManager.NavigateTo(RoutingConstants.ENTERPRISE_HOME);
        }

        public static void NavigateToSearchDogTraining(this NavigationManager navigationManager, InstructorCategory? category = null, DayOfWeek? dayOfWeek = null, TimeSpan? after = null, TimeSpan? before = null, int? trainerId = null)
        {
            string categoryParam = category is null ? null : "category="+category.ToString();
            string dayParam = dayOfWeek is null ? null : "day="+dayOfWeek.ToString();
            string afterParam = after is null ? null : "after=" + after?.ToQueryParameterString();
            string beforeParam = before is null ? null : "before=" + before?.ToQueryParameterString();
            string trainerParam = trainerId is null ? null : "trainerId=" + trainerId.Value;

            string queryParameters = string.Join("&", (new string[] { categoryParam, dayParam, afterParam, beforeParam, trainerParam}).Where(s => !string.IsNullOrEmpty(s)));

            navigationManager.NavigateTo($"{RoutingConstants.SEARCH_DOG_TRAINING}?{queryParameters}");
        }

        public static void NavigateToSearchDogTrainer(this NavigationManager navigationManager, InstructorCategory? category = null)
        {
            string categoryParam = category is null ? null : "category=" + category.ToString();

            string queryParameters = string.Join("&", (new string[] { categoryParam }).Where(s => !string.IsNullOrEmpty(s)));

            navigationManager.NavigateTo($"{RoutingConstants.SEARCH_DOG_TRAINER}?{queryParameters}");
        }

    }
}
