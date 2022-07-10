using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace UnitTests
{
    public class TestBase : IDisposable
    {
        protected readonly TestDBContext _context;

        public TestBase()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DoggoWorldUnitTest");
            _context = new TestDBContext(builder.Options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
