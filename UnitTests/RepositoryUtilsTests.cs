using ModelLib.Repositories;
using Microsoft.EntityFrameworkCore;
using EntityLib.Entities.Identity;
using ModelLib.ApiDTOs.Pagination;

namespace UnitTests
{
    public class RepositoryUtilsTests : TestBase
    {

        [Theory]
        [InlineData(0, "A", -1, "", 10, true)]
        [InlineData(1, "A", 10, "A", 7, true)]
        [InlineData(2, "A", 7, "AB", 8, true)]
        [InlineData(3, "A", 8, "AB", 9, false)]
        public async Task GetPaginationQuery_Returns_Expected_Pagination_Results(int page, string searchString, int lastId, string lastString, int expectedUserId, bool expectedHasNext)
        {
            var paginationRequest = new StringPaginationRequest() { ItemsPerPage = 1 ,Page = page, LastId = lastId, LastString = lastString};
            var query = _context.Users.Where(u => u.FullNameNormalized.StartsWith(searchString));

            var orderByFunc = (IQueryable<ApplicationUser> query) =>
                query.OrderBy(u => u.FullNameNormalized).ThenBy(u => u.Id);

            var keyOffsetFunc = (IQueryable<ApplicationUser> query) =>
                query.Where(u => u.FullNameNormalized.CompareTo(paginationRequest.LastString) > 0 || 
                (u.FullNameNormalized.CompareTo(paginationRequest.LastString) == 0 && u.Id > paginationRequest.LastId)
            );

            var (actualPaginationResult, actualQuery) = await RepositoryUtils.GetPaginationQuery(query, orderByFunc, keyOffsetFunc, paginationRequest);
            
            var users = await actualQuery.ToListAsync();
            
            Assert.Equal(expectedHasNext, actualPaginationResult.HasNext);
            Assert.Equal(expectedUserId, users.First().Id);
            Assert.Equal(paginationRequest.Page, actualPaginationResult.CurrentPage);
            Assert.Equal(paginationRequest.ItemsPerPage, users.Count);
        }
    }
}
