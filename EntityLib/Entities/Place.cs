using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using EntityLib.Entities.Constants;
using Microsoft.EntityFrameworkCore;
using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.PostsAndComments;
using EntityLib.Entities.Identity;
using EntityLib.Entities.Chatting;
using EntityLib.Entities.EventsAndMeetups;

namespace EntityLib.Entities
{
    [Index(nameof(Name))]
    public class Place : EntityWithLocation
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "geometry (point)")]
        public Point Location { get; set; }

        [Required]
        public Enums.PlaceType PlaceType { get; set; }

        public Enums.Interests? PlaceSubType { get; set; }

        [Required]
        [MaxLength(PlaceConstants.NameMaxLength)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(PlaceConstants.DescriptionMaxLength)]
        public string? Description { get; set; }
        public string[]? ImageUrls { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public int? CreatedById { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public ICollection<CheckIn>? CheckIns { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public Company? Company { get; set; }

        public ICollection<Post>? Posts { get; set; }
        public ICollection<Chat>? Chats { get; set; }
        public ICollection<Gathering> Meetups { get; set; }
        public ICollection<Gathering> MeetupReferences { get; set; }
    }
}
