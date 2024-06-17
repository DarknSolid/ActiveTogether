using ModelLib.DTOs;

namespace ModelLib.ApiDTOs.Pagination
{
    public class DistancePaginationRequest : PaginationRequest
    {
        public float PreviousDistance { get; set; }
        public SearchAreaDTO SearchArea { get; set; }
    }
}
