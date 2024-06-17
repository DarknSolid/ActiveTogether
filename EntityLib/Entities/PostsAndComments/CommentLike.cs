using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EntityLib.Entities.PostsAndComments
{
    public class CommentLike : LikeBase
    {
        [Required]
        [ForeignKey(nameof(PostComment))]
        [Column("CommentId")]
        override public int TargetId { get; set; }

        [ForeignKey(nameof(TargetId))]
        public PostComment Comment { get; set; }
    }
}
