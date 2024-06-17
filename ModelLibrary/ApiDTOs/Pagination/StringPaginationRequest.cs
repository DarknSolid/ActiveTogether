using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.ApiDTOs.Pagination
{
    public class StringPaginationRequest : PaginationRequest
    {
        public string LastString { get; set; }
    }
}
