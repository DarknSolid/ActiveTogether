using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.ApiDTOs
{
    public class GetCheckInListDTO
    {
        public PaginationRequest PaginationRequest { get; set; }
        public int PlaceId { get; set; }
        public bool OnlyActiveCheckIns { get; set; }
    }
}
