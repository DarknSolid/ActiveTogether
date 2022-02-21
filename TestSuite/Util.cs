using ModelLib.DTOs.DogPark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSuite
{
    public class Util
    {
        public static DogParkCreateDTO GenerateDogPark()
        {
            Random random = new Random();
            var lat = random.NextDouble() * 100;
            var lon = random.NextDouble() * 100;
            return new DogParkCreateDTO
            {
                Description = "",
                Name = "",
                Latitude = (float)lat,
                Longitude = (float)lon
            };
        }
    }
}
