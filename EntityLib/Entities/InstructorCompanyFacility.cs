using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities
{
    public class InstructorCompanyFacility
    {
        [ForeignKey(nameof(InstructorCompany))]
        public int InstructorCompanyId { get; set; }
        public InstructorFacility InstructorFacility { get; set; }

        public Company InstructorCompany { get; set; }
    }
}
