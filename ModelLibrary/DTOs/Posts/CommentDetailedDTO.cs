
using EntityLib.Entities.AbstractClasses;

namespace ModelLib.DTOs.Posts
{
    public class CommentDetailedDTO : DateAndIntegerId
    {
        override public int Id { get; set; }
        override public DateTime DateTime { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public IList<LikeDetailedDTO> Likes { get; set; } = new List<LikeDetailedDTO>();
        public string UserImageUrl { get; set; }
    }
}
