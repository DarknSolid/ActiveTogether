using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities.PostsAndComments
{
    public class Post : DateAndIntegerId
    {
        [Key]
        override public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Place))]
        public int? PlaceId { get; set; }

        public string? Body { get; set; }

        public string[]? Media { get; set; }

        public PostArea? Area { get; set; }
        public PostCategory? Category { get; set; }

        [ForeignKey(nameof(Entities.Dog))]
        public int? DogId { get; set; }

        public ICollection<PostLike>? PostLikes { get; set; }
        public ICollection<PostComment>? PostComments { get; set; }


        [Required]
        [Column("CreatedAt")]
        override public DateTime DateTime { get; set; }

        public ApplicationUser User { get; set; }
        public Place? Place { get; set; }
        public Dog? Dog { get; set; }
    }
}
