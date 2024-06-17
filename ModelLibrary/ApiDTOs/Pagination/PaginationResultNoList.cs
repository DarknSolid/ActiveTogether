
namespace ModelLib.ApiDTOs.Pagination
{
    public class PaginationResultNoList
    {
        public int CurrentPage { get; set; }
        public bool HasNext { get; set; }
        public int Total { get; set; }
    }
}
