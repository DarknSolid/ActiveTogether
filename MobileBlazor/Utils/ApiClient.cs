using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ModelLib.Constants;
using WebApp.DTOs.Authentication;
using System.Net.Http.Json;
using RazorLib.Interfaces;
using ModelLib.DTOs;
using ModelLib.ApiDTOs;
using ModelLib.DTOs.DogPark;
using Newtonsoft.Json;
using RazorLib.Models;

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
        public async Task<MapSearchResultDTO> FetchMapMarkers(MapSearchDTO mapSearchDTO)
        {
            var url = _baseApiUrl + ApiEndpoints.POST_MAP_POINTS;
            var result = await _httpClient.PostAsJsonAsync(url, mapSearchDTO);
            var str = await result.Content.ReadAsStringAsync();
            var thing = JsonConvert.DeserializeObject<MapSearchResultDTO>(str);
            return thing;
        }

        public async Task<DogParkDetailedDTO> GetDogPark(int id)
        {
            return await _httpClient.GetFromJsonAsync<DogParkDetailedDTO>(_baseApiUrl + ApiEndpoints.GET_DOG_PARK + id);
        }

        public Task<(PaginationResult, List<RatingDTO>)> GetDogParkRatings(int id, int page, int pageCount)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
