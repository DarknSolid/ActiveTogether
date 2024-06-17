namespace ModelLib.ApiDTOs.Pagination
{
    public class PaginationResult<T> : PaginationResultNoList
    {
        public int LastId { get; set; }
        public IList<T> Result { get; set; }
    }
}
