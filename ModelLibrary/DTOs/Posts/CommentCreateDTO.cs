using EntityLib.Entities.Constants;
using System.ComponentModel.DataAnnotations;

namespace ModelLib.DTOs.Posts
{
    public class CommentCreateDTO
    {
        public int PostId { get; set; }

        [Required]
        [MaxLength(PostConstants.MaxContentLength)]
        public string Content { get; set; }
    }
}
