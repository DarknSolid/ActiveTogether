using ModelLib.ApiDTOs.Pagination;

namespace ModelLib.DTOs.Posts
{
    public class PostDateTimePaginationRequest : DateTimePaginationRequest
    {
        public PostFilter? Filter { get; set; }
    }
}
