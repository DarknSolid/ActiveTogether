using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs.DogPark
{
    public class DogParkRequestDetailedDTO : DogParkRequestCreateDTO
    {
        public DateTime RequestDate { get; set; }
        public int Id { get; set; }
    }
}
