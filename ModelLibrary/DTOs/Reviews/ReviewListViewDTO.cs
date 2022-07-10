using ModelLib.ApiDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs.Reviews
{
    public class ReviewListViewDTO
    {
        public PaginationResult PaginationResult { get; set; }
        public List<ReviewDetailedDTO> Reviews { get; set; }
    }
}
