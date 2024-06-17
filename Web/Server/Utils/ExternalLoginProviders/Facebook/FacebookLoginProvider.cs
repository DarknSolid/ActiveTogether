using Newtonsoft.Json;

namespace Web.Server.Utils.ExternalLoginProviders.Facebook
{
    public interface IFacebookLoginProvider
    {
        public string Provider { get; set; }
        public Task<bool> ValidateAccessToken(string accessToken);
        public Task<FacebookUserInfo> GetUserInfo(string accessToken);
    }

    public class FacebookLoginProvider : IFacebookLoginProvider
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,picture,email&access_token={0}";
        private readonly FacebookConfig _facebookConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        public string Provider { get; set; }

        public FacebookLoginProvider(IHttpClientFactory httpClientFactory, FacebookConfig facebookConfig)
        {
            _httpClientFactory = httpClientFactory;
            _facebookConfig = facebookConfig;
            Provider = "Facebook";
        }

        public async Task<bool> ValidateAccessToken(string accessToken)
        {
            var formattedUrl = string.Format(
                TokenValidationUrl, 
                accessToken, 
                _facebookConfig.AppId, 
                _facebookConfig.AppSecret);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            if (result.IsSuccessStatusCode)
            {
                var responseString = await result.Content.ReadAsStringAsync();
                var model = FacebookTokenValidationResult.FromJson(responseString);

                return model?.Data?.IsValid ?? false;
            }
            return false;
        }  

        public async Task<FacebookUserInfo> GetUserInfo(string accessToken)
        {
            var formattedUrl = string.Format(UserInfoUrl, accessToken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            var responseString = await result.Content.ReadAsStringAsync();
            var model = FacebookUserInfo.FromJson(responseString);
            return model;
        }
    }
}
