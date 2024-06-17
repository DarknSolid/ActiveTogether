using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Constants;
using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities.PostsAndComments
{
    public class PostComment : DateAndIntegerId
    {
        [Key]
        override public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [Required]
        [StringLength(PostConstants.MaxContentLength)]
        public string Text { get; set; }

        [Required]
        [Column("CreatedAt")]
        override public DateTime DateTime { get; set; }

        public ICollection<CommentLike>? CommentLikes { get; set; }

        public ApplicationUser User { get; set; }
        public Post Post { get; set; }
    }
}
