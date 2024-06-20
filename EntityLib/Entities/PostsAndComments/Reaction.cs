using EntityLib.Entities.Chatting;
using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.PostsAndComments.Enums;

namespace EntityLib.Entities.PostsAndComments
{
    public class Reaction
    {
        [Required]
        public ReactionTargetType TargetType { get; set; }

        [Required]
        public int TargetId { get; set; }


        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [Required]
        public ReactionType ReactionType { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; }
        
        public Post? Post { get; set; }
        public PostComment? Comment { get; set; }
        public Message? Message { get; set; }

    }
}
