using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.ApiDTOs.Pagination
{
    public class PaginationRequest
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; } = 10;
        public int LastId { get; set; }
    }
}
