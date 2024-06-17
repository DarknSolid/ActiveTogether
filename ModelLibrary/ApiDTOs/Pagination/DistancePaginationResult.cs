
namespace ModelLib.ApiDTOs.Pagination
{
    public class DistancePaginationResult<T> : PaginationResult<T>
    {

        public DistancePaginationResult()
        {

        }

        public DistancePaginationResult(PaginationResult<T> paginationBase)
        {
            HasNext = paginationBase.HasNext;
            CurrentPage = paginationBase.CurrentPage;
            LastId = paginationBase.LastId;
            Total = paginationBase.Total;
            Result = paginationBase.Result;
        }
        public float LastDistance { get; set; }
    }
}
