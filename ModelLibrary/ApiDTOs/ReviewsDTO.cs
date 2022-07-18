
namespace ModelLib.ApiDTOs
{
    public class ReviewsDTO
    {
        public PaginationRequest PaginationRequest { get; set; } = new();
        public int PlaceId { get; set; }
    }
}
