using ModelLib.DTOs.Authentication;

namespace ModelLib.ApiDTOs
{
    public class SignInWithThirdPartyResult
    {
        public bool Success { get; set; }
        public bool DidRegisterNewUser { get; set; }
        public UserDetailedDTO? UserInfo { get; set; }
        public IList<string> Errors { get; set; }
    }
}
