using ModelLib.Constants;
using WebApp.DTOs.Authentication;
using System.Net.Http.Json;
using RazorLib.Interfaces;
using ModelLib.DTOs;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.DogPark;
using Newtonsoft.Json;
using ModelLib.DTOs.Reviews;
using static EntityLib.Entities.Enums;
using ModelLib.DTOs.Dogs;

namespace MobileBlazor.Utils
{
    public class ApiClient : IMobileApiClient, IApiClient
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        private UserInfoDTO _userInfo;

        public ApiClient(HttpClient httpClient, string baseApiUrl)
        {
            _httpClient = httpClient;
            _baseApiUrl = baseApiUrl;
        }

        #region IMobileApiClient implementation:

        public async Task<UserInfoDTO> FacebookLogin()
        {
            string accessToken = "";

#if __ANDROID__
            Xamarin.Facebook.Login.LoginManager.Instance.LogInWithReadPermissions(MainActivity.Instance, new List<string> { "public_profile", "email" });
            while (MainActivity.Instance.FacebookAccessToken == null)
            {
                await Task.Delay(500);
            }
            accessToken = MainActivity.Instance.FacebookAccessToken;
#endif

            var url = _baseApiUrl + ApiEndpoints.POST_FACEBOOK_LOGIN;
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, new FacebookLoginDTO() { AccessToken = accessToken });
                if (response.IsSuccessStatusCode)
                {
                    return await GetUserInfo();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public async Task<UserInfoDTO> GetUserInfo()
        {
            if (_userInfo == null)
            {
                var url = _baseApiUrl + ApiEndpoints.GET_USER_INFO;
                return await _httpClient.GetFromJsonAsync<UserInfoDTO>(url);
            }
            return _userInfo;
        }

        public async Task<UserInfoDTO> Login(string email, string password)
        {
            var login = new LoginDTO() { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync(_baseApiUrl + ApiEndpoints.POST_LOGIN, login);
            return await GetUserInfo();
        }

        public async Task Logout()
        {
            await _httpClient.GetAsync(_baseApiUrl + ApiEndpoints.GET_LOG_OUT);
        }

        #endregion

        #region BlazorLib Interface implementation:
        public async Task<MapSearchResultDTO> FetchMapMarkersAsync(MapSearchDTO mapSearchDTO)
        {
            var url = _baseApiUrl + ApiEndpoints.POST_MAP_POINTS;
            return await PostAsync<MapSearchDTO, MapSearchResultDTO>(url, mapSearchDTO);
        }

        public async Task<DogParkDetailedDTO> GetDogParkAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<DogParkDetailedDTO>(_baseApiUrl + ApiEndpoints.GET_DOG_PARK + id);
        }

        public async Task<ReviewListViewDTO> GetReviewsAsync(int id, FacilityType reviewType, int page, int pageCount)
        {
            var url = _baseApiUrl + ApiEndpoints.POST_LIST_REVIEWS;
            var body = new ReviewsDTO
            {
                RevieweeId = id,
                ReviewType = reviewType,
                PaginationRequest = new PaginationRequest
                {
                    Page = page,
                    ItemsPerPage = pageCount
                }
            };

            return await PostAsync<ReviewsDTO, ReviewListViewDTO>(url, body);
        }

        public async Task<ReviewCreatedDTO> CreateReviewAsync(FacilityType reviewType, ReviewCreateDTO reviewCreateDTO)
        {
            var url = _baseApiUrl + ApiEndpoints.POST_CREATE_REVIEW;
            return await PostAsync<ReviewCreateDTO, ReviewCreatedDTO>(url, reviewCreateDTO);
        }

        private async Task<O> PostAsync<I, O>(string url, I body)
        {
            var result = await _httpClient.PostAsJsonAsync(url, body);
            var str = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<O>(str);
        }

        public async Task<List<DogListDTO>> GetMyDogsAsync()
        {
            var user = await GetUserInfo();
            return await GetDogsAsync(user.Id);
        }

        public async Task<DogDetailedDTO> GetDogDetailedAsync(int id)
        {
            var url = _baseApiUrl + ApiEndpoints.GET_DETAILED_DOG + id;
            return await _httpClient.GetFromJsonAsync<DogDetailedDTO>(url);
        }

        public async Task<int?> CreateDogAsync(DogCreateDTO dto)
        {
            var url = _baseApiUrl + ApiEndpoints.POST_CREATE_DOG;
            return await PostAsync<DogCreateDTO, int>(url, dto);
        }

        public async Task<IDictionary<int, string>> GetDogBreedsAsync()
        {
            var url = _baseApiUrl + ApiEndpoints.GET_DOG_BREEDS;
            return await _httpClient.GetFromJsonAsync<IDictionary<int, string>>(url);
        }

        public async Task UpdateDogAsync(DogUpdateDTO dto)
        {
            var url = _baseApiUrl + ApiEndpoints.Update_DOG;
            await _httpClient.PutAsJsonAsync(url, dto);
        }

        public async Task<List<DogListDTO>> GetDogsAsync(int userId)
        {
            var url = _baseApiUrl + ApiEndpoints.GET_LIST_DOGS + userId;
            return await _httpClient.GetFromJsonAsync<List<DogListDTO>>(url);
        }

        public async Task DeleteDogAsync(int dogId)
        {
            var url = _baseApiUrl + ApiEndpoints.DELETE_DOG + dogId;
            await _httpClient.DeleteAsync(url);
        }

        #endregion
    }
}
