using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace EntityLib.Entities
{
    public class Friendship : IntegerId
    {
        [Key]
        override public int Id { get; set; }
        [Required]
        public int RequesterId { get; set; }
        [Required]
        public int RequesteeId { get; set; }

        [Required]
        public bool IsAccepted { get; set; }

        public ApplicationUser Requester { get; set; }
        public ApplicationUser Requestee { get; set; }
    }
}
