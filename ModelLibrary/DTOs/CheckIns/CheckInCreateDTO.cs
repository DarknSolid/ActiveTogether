using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.CheckIns
{
    public class CheckInCreateDTO
    {
        public FacilityType FacilityType { get; set; }
        public int FacilityId { get; set; }
        public List<int> DogsToCheckIn { get; set; }
    }
}
