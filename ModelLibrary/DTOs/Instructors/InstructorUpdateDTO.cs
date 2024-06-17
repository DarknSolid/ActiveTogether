using EntityLib.Entities.Constants;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Instructors
{
    public class InstructorUpdateDTO : InstructorCreateDTO
    {
        public int InstructorCompanyId { get; set; }

    }
}
