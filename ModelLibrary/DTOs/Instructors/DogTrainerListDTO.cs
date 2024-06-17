using ModelLib.DTOs.Places;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Instructors
{
    public class DogTrainerListDTO : PlaceListDTO
    {
        public double? Rating { get; set; }
        public int RatingCount { get; set; }
        public string? CoverImgUrl { get; set; }
        public InstructorCategory[] Categories { get; set; }
    }
}
