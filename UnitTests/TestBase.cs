using Entities;
using Microsoft.EntityFrameworkCore;

namespace UnitTests
{
    public class TestBase : IDisposable
    {
        protected TestDBContext _context;

        public TestBase()
        {
            // Necessary to ensure that each unit test tests on a fresh dababase:
            var dbName = Guid.NewGuid().ToString();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName);
            _context = new TestDBContext(builder.Options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
