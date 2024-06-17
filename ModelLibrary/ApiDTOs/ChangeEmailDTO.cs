
namespace ModelLib.ApiDTOs
{
    public class ChangeEmailDTO
    {
        public string CurrentEmail { get; set; }
        public string NewEmail { get; set; }
        public string Token { get; set; }
    }
}
