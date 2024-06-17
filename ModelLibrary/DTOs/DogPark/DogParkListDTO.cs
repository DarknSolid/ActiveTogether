using ModelLib.DTOs.Places;

namespace ModelLib.DTOs.DogPark
{
    public class DogParkListDTO : PlaceListDTO
    {
        public DateTime DateAdded { get; set; }
        public int RatingCount { get; set; }
        public float Rating { get; set; }
    }
}