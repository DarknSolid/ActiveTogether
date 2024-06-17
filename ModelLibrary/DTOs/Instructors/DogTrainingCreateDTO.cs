using EntityLib.Entities.Identity;
using EntityLib.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;
using FisSst.BlazorMaps;
using ModelLib.ApiDTOs;

namespace ModelLib.DTOs.Instructors
{
#nullable disable
    public class DogTrainingCreateDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        [Url(ErrorMessage = "Invalid url")]
        public string OriginalDogWebsiteUrl { get; set; }

        public int AssignedInstructorId { get; set; }

        public float Price { get; set; }

        public LatLng Location { get; set; }

        [Required]
        public DateTime RegistrationDeadline { get; set; }

        public TrainingTime[] TrainingTimes { get; set; }

        public int MaxParticipants { get; set; }
        public InstructorCategory Category { get; set; }

        public FileDetailedDTO? CoverImage { get; set; }
    }

    public class TrainingTime
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
