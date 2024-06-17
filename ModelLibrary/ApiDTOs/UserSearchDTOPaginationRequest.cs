using ModelLib.ApiDTOs.Pagination;

namespace ModelLib.ApiDTOs
{
    public class UserSearchDTOPaginationRequest
    {
        public StringPaginationRequest PaginationRequest { get; set; }
        public string SearchString { get; set; }
    }
}
