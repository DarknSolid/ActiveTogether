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
        public int Id { get; set; }

        [Required]
        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateTime { get; set; }

        [Required]
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [Required]
        [StringLength(PostConstants.MaxContentLength)]
        public string Text { get; set; }

        public ICollection<Reaction>? Reactions { get; set; }
        public ApplicationUser User { get; set; }
        public Post Post { get; set; }
    }
}
