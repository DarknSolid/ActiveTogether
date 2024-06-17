using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities.PostsAndComments
{
    public class PostLike : LikeBase
    {
        [Required]
        [ForeignKey(nameof(Post))]
        [Column("PostId")]
        override public int TargetId { get; set; }
        
        public Post Post { get; set; }

    }
}
