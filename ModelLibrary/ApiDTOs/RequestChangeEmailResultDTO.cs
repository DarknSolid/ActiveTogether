
namespace ModelLib.ApiDTOs
{
    public class RequestChangeEmailResultDTO
    {
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
    }
}
