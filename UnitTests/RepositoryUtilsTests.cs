using ModelLib.ApiDTOs;
using ModelLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using WebApp.Entities;

namespace UnitTests
{
    public class RepositoryUtilsTests : IDisposable
    {

        private readonly TestDBContext _context;

        public RepositoryUtilsTests()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DoggoWorldUnitTest");
            _context = new TestDBContext(builder.Options);
            _context.Database.EnsureCreated();
        }

        [Theory]
        [InlineData(1, 0, 1, 0, true, 1)]
        [InlineData(1, 1, 2, 1, true, 1)]
        [InlineData(1, 2, 3, 2, false, 1)]
        [InlineData(10, 0, 1, 0, false, 3)]
        [InlineData(10, 1, -1, 1, false, 0)]
        public async Task GetPaginationQuery_behaves(int itemsPerPage, int page, int expectedId, int expectedPage, bool expectedHasNext, int expectedCount)
        {
            var paginationRequest = new PaginationRequest() { ItemsPerPage = itemsPerPage ,Page = page};
            var query = _context.DogParks;

            var (actualPaginationResult, actualQuery) = await RepositoryUtils.GetPaginationQuery(query, paginationRequest);
            var dogParks = await actualQuery.ToListAsync();
            Assert.Equal(expectedHasNext, actualPaginationResult.HasNext);
            Assert.Equal(expectedPage, actualPaginationResult.CurrentPage);
            Assert.Equal(expectedId, dogParks.FirstOrDefault()?.Id ?? -1);
            Assert.Equal(expectedCount, dogParks.Count);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
