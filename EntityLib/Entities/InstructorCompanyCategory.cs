using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities
{
    public class InstructorCompanyCategory
    {
        [ForeignKey(nameof(InstructorCompany))]
        public int InstructorCompanyId { get; set; }
        public InstructorCategory InstructorCategory { get; set; }

        public Company InstructorCompany { get; set; }

    }
}
