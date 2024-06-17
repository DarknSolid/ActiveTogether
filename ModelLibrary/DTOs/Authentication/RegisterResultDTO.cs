
namespace ModelLib.DTOs.Authentication
{
    public class RegisterResultDTO
    {
        public bool Success { get; set; }
        public bool MustConfirmEmail { get; set; }
        public UserDetailedDTO? UserInfo { get; set; }
        public IList<string> ErrorMessages { get; set; }
    }
}
