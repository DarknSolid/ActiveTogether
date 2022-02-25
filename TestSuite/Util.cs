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
            var lat = random.Next(55,57) + random.NextDouble();
            var lon = random.Next(11,14) + random.NextDouble();
            return new DogParkCreateDTO
            {
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur sagittis eros porta lectus sagittis iaculis. Aliquam erat volutpat. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vestibulum eget mattis velit. Etiam vulputate metus et mauris sodales, eget vulputate nulla laoreet. Nullam consequat convallis lorem a bibendum. Interdum et malesuada fames ac ante ipsum primis in faucibus.",
                Name = "Parken",
                Latitude = (float) lat,
                Longitude = (float) lon
            };
        }
    }
}
