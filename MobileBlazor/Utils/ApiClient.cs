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

namespace MobileBlazor.Utils
{
    public class ApiClient : IApiClient
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public ApiClient(HttpClient httpClient, string baseApiUrl)
        {
            _httpClient = httpClient;
            _baseApiUrl = baseApiUrl;
        }

        public async Task<UserInfoDTO> FacebookLogin()
        {
            string accessToken = "";

            #if __ANDROID__
                Xamarin.Facebook.Login.LoginManager.Instance.LogInWithReadPermissions(MainActivity.Instance, new List<string> { "public_profile", "email" });
                while (MainActivity.Instance.FacebookAccessToken == null)
                {
                    await Task.Delay(1000);
                    Console.WriteLine("Sleeping...");
                }
                accessToken = MainActivity.Instance.FacebookAccessToken;
            #endif

            var url = _baseApiUrl + ApiEndpoints.FACEBOOK_LOGIN;
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, new FacebookLoginDTO() { AccessToken = accessToken });
                if (response.IsSuccessStatusCode)
                {
                    url = _baseApiUrl + ApiEndpoints.USER_INFO;
                    return await _httpClient.GetFromJsonAsync<UserInfoDTO>(url);
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public async Task Logout()
        {
            await _httpClient.GetAsync(_baseApiUrl + ApiEndpoints.LOG_OUT);
        }
    }
}
