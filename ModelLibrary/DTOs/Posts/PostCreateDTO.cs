using ModelLib.ApiDTOs;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Posts
{
    public class PostCreateDTO
    {
        public string? Body { get; set; }
        public FileListDTO[]? Media { get; set; }
        public PostArea? Area { get; set; }
        public PostCategory? Category { get; set; }
        public int? PlaceId { get; set; }
        public int? DogId { get; set; }
    }
}
