namespace ModelLib.ApiDTOs.Pagination
{
    public class DateTimePaginationResult<T> : PaginationResult<T>
    {
        public DateTime LastDate { get; set; }

        public DateTimePaginationResult()
        {
            
        }
        public DateTimePaginationResult(PaginationResult<T> paginationResult)
        {
            base.CurrentPage = paginationResult.CurrentPage;
            base.LastId = paginationResult.LastId;
            base.Total = paginationResult.Total;
            base.HasNext = paginationResult.HasNext;
            base.Result = paginationResult.Result;
        }
    }
}
