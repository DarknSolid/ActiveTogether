using ModelLib.DTOs.Authentication;

namespace MobileBlazor.Utils
{
    public interface IMobileApiClient
    {
        public Task<UserDetailedDTO> FacebookLogin();
        public Task<UserDetailedDTO> GetUserInfo();
        public Task<UserDetailedDTO> Login(string email, string password);
        public Task Logout();
        
    }
}
