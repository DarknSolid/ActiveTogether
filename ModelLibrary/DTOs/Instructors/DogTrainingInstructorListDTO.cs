using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Instructors
{
    /// <summary>
    /// This dto is intended for when an instructor wants to see his own courses.
    /// This has a bit different information than the "public" list dto
    /// </summary>
    public class DogTrainingInstructorListDTO
    {
        public int DogTrainingId { get; set; }
        public string ImageUri { get; set; }
        public string Title { get; set; }
        public InstructorCategory Category { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public DateTime FirstTrainingDate { get; set; }
        public float Price { get; set; }
        public int MaxParticipants { get; set; }
    }
}
