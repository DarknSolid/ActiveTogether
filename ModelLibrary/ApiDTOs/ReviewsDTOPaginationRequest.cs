
using ModelLib.ApiDTOs.Pagination;

namespace ModelLib.ApiDTOs
{
    public class ReviewsDTOPaginationRequest
    {
        public DateTimePaginationRequest PaginationRequest { get; set; } = new();
        public int PlaceId { get; set; }
    }
}
