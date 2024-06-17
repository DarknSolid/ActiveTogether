using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Posts
{
    public class PostFilter
    {
        public int? UserId { get; set; }
        public PostArea? Area { get; set; }
        public PostCategory? Category { get; set; }
        public int? PlaceId { get; set; }
        public bool IncludePlaceDetails { get; set; }
        public int? DogId { get; set; }
        public DogRace? DogRace { get; set; }
    }
}
