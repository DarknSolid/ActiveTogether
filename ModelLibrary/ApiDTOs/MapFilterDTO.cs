using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.ApiDTOs
{
    public class MapFilterDTO
    {
        public DogParkFilterDTO DogParkFilterDTO { get; set; }

        public MapFilterDTO()
        {
            DogParkFilterDTO = new DogParkFilterDTO();
        }
    }
}
