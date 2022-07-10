using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Repositories
{
    public class RepositoryEnums
    {
        public enum ResponseType
        {
            Conflict,
            Updated,
            Created,
            NotFound
        }
    }
}
