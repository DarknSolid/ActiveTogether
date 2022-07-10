using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.ApiDTOs
{
    public class ReviewsDTO
    {
        public PaginationRequest PaginationRequest { get; set; } = new();
        public int RevieweeId { get; set; }
        public ReviewType ReviewType { get; set; }
    }
}
