using ModelLib.ApiDTOs.Pagination;

namespace ModelLib.ApiDTOs
{
    public class CheckInListDTOPaginationRequest
    {
        public DateTimePaginationRequest PaginationRequest { get; set; }
        public int PlaceId { get; set; }
        public bool OnlyActiveCheckIns { get; set; }
    }
}
