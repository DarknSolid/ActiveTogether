
using EntityLib.Entities.AbstractClasses;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Posts
{
    public class PostDetailedDTO : DateAndIntegerId
    {
        override public int Id { get; set; }
        public int UserId { get; set; }
        public FacilityType? UserProfession { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string? UserImageUrl { get; set; }
        override public DateTime DateTime { get; set; }
        public string? Body { get; set; }
        public string[]? MediaUrls { get; set; }
        public PostArea? Area { get; set; }
        public PostCategory? Category { get; set; }
        public int? PlaceId { get; set; }
        public string? PlaceName { get; set; }
        public string? PlaceImageUrl { get; set; }
        public FacilityType? PlaceFacilityType { get; set; }
        public int TotalComments { get; set; }
        public IList<LikeDetailedDTO> Likes { get; set; } = new List<LikeDetailedDTO>();
    }
}
