using ModelLib.DTOs.AbstractDTOs;
using NpgsqlTypes;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Instructors
{
#nullable disable
    public class DogTrainingListDTO : DTOWithLocation
    {
        public int TrainingTimes { get; set; }
        public int InstructorCompanyId { get; set; }
        override public int Id { get; set; }
        override public NpgsqlPoint Location { get; set; }
        public string InstructorCompanyLogoUri { get; set; }
        public string InstructorCompanyName { get; set; }
        public string Title { get; set; }
        public InstructorCategory Category { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public DateTime FirstTrainingDate { get; set; }
        public float Price { get; set; }
        public int MaxParticipants { get; set; }
        public float? DistanceMeters { get; set; }
        public string? CoverImgUrl { get; set; }
    }
}
