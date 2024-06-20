using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EntityLib.Entities.Matching
{
    public class Like : DateAndIntegerId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateTime { get; set; }

        [Required]
        public bool IsIndividualLike { get; set; }

        [Required]
        public int TargetOneId { get; set; }
        [Required]
        public int TargetTwoId { get; set; }

        [Required]
        public bool IsAccepted { get; set; }

        public ApplicationUser Requester { get; set; }
        public ApplicationUser Requestee { get; set; }
    }
}
