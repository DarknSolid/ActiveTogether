using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;

namespace ModelLib.Repositories
{
    public class RepositoryUtils
    {
        public static async Task<(PaginationResult, IQueryable<T>)> GetPaginationQuery<T>(IQueryable<T>? query, PaginationRequest pagReq)
        {
            var startIndex = pagReq.Page * pagReq.ItemsPerPage;
            var total = await query.CountAsync();
            var hasNext = startIndex + pagReq.ItemsPerPage < total;

            query = query.Skip(startIndex).Take(pagReq.ItemsPerPage);

            var paginationResult = new PaginationResult() 
            { 
                CurrentPage = pagReq.Page, 
                HasNext = hasNext 
            };

            return (paginationResult, query);
        }
    }
}
