using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.ApiDTOs
{
    public class PaginationResult
    {
        public int CurrentPage { get; set; }
        public bool HasNext { get; set; }
    }
}
