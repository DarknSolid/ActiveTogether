using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Identity;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities
{
#nullable disable
    public class DogTraining : EntityWithLocation
    {
        [Key]
        override public int Id { get; set; }

        [Required]
        [Column(TypeName = "geometry (point)")]
        override public Point Location { get; set; }

        [ForeignKey(nameof(Company))]
        public int InstructorCompanyId { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public int AssignedInstructorId { get; set; }

        public string OriginalTrainingWebsiteUri { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public DateTime RegistrationDeadline { get; set; }

        [Required]
        public DateTime[] TrainingTimes { get; set; }

        [Required]
        public int MaxParticipants { get; set; }
        [Required]
        public InstructorCategory Category { get; set; }

        public Company InstructorCompany { get; set; }
        public ApplicationUser AssignedInstructor { get; set; }
        public string? CoverImgageBlobUrl { get; set; }
    }
}
