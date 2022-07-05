using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTOs.Authentication;

namespace MobileBlazor.Utils
{
    public interface IMobileApiClient
    {
        public Task<UserInfoDTO> FacebookLogin();
        public Task<UserInfoDTO> GetUserInfo();
        public Task<UserInfoDTO> Login(string email, string password);
        public Task Logout();
        
    }
}
