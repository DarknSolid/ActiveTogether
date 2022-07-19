using EntityLib;

namespace ModelLib.Repositories
{
    public class RepositoryBase
    {
        protected readonly IApplicationDbContext _context;

        public RepositoryBase(IApplicationDbContext context)
        {
            _context = context;
        }
    }
}
