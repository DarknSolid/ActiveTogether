using ModelLib.ApiDTOs.Pagination;

namespace ModelLib.DTOs.Posts
{
    public class CommentGetRequest : DateTimePaginationRequest
    {
        public int PostId { get; set; }
    }
}
