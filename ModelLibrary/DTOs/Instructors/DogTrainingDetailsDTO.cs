using FisSst.BlazorMaps;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Instructors
{
#nullable disable
    public class DogTrainingDetailsDTO
    {
        public string Description { get; set; }
        public int InstructorCompanyId { get; set; }
        public double InstructorRating { get; set; }
        public int InstructorRatingCount { get; set; }
        public TrainingTime[] TrainingTimes { get; set; }
        public LatLng Location { get; set; }
        public string OriginalDogTrainingWebsiteUri { get; set; }
        public int DogTrainingId { get; set; }
        public string InstructorCompanyLogoUri { get; set; }
        public string InstructorCompanyName { get; set; }
        public string Title { get; set; }
        public InstructorCategory Category { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public float Price { get; set; }
        public int MaxParticipants { get; set; }
        public string? CoverImgUrl { get; set; }

    }
}
