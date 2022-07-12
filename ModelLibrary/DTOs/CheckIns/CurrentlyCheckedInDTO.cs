using ModelLib.DTOs.Dogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.CheckIns
{
    public class CurrentlyCheckedInDTO
    {
        public List<DogListDTO> Dogs { get; set; }
        public int FacilityId { get; set; }
        public FacilityType FacilityType { get; set; }
        public DateTime CheckInDate { get; set; }
    }
}
