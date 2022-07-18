using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    [Table(nameof(Friends))]
    public  class Friends
    {
        //Composite primary key
        [Required]
        public int UserId { get; set; }
        [Required]
        public int FriendId { get; set; }

        [Required]
        public bool IsAccepted { get; set; }

        public ApplicationUser User { get; set; }
        public ApplicationUser Friend { get; set; }
    }
}
