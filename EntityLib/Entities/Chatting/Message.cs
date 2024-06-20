using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Constants;
using EntityLib.Entities.Identity;
using EntityLib.Entities.PostsAndComments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities.Chatting
{
    public class Message : DateAndIntegerId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateTime { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [StringLength(PostConstants.MaxContentLength)]
        public string? Text { get; set; }
        public string[]? MediaUrls { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
        public ICollection<ChatMember> LastReadMembers { get; set; }

    }
}
