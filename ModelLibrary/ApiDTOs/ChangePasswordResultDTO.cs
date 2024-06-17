
namespace ModelLib.ApiDTOs
{
    public class ChangePasswordResultDTO
    {
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
    }
}
