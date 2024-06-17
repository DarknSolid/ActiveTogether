
namespace ModelLib.DTOs.Posts
{
    public class PostCreateResult
    {
        public bool Success { get; set; }
        public int? PostId { get; set; }
        public string[]? BlobUploadUrls { get; set; }
    }
}
