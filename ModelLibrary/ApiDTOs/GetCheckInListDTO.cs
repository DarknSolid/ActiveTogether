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
        public int FacilityId { get; set; }
        public FacilityType FacilityType { get; set; }
        public bool OnlyActiveCheckIns { get; set; }
    }
}
