using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Chatting;
using EntityLib.Entities.EventsAndMeetups;
using EntityLib.Entities.Matching;
using EntityLib.Entities.PostsAndComments;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public string FullNameNormalized { get; set; }

        [Required]
        public DateTime Birth { get; set; }
        // is either a blob URL or third party url, from e.g. Facebook or Google
        public string? ProfileImageUrl { get; set; }

        [Required]
        public bool AtLeastThirteenYearsOld { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        public Interests[]? Interests { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public ICollection<Place> CreatedPlaces { get; set; }
        public ICollection<CheckIn> CheckIns { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Friendship> Friends { get; set; }
        public ICollection<Company> Companies { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<PostComment> Comments { get; set; }
        public ICollection<ChatMember> ChatMembers { get; set; }
        public ICollection<Match> MatchOne{ get; set; }
        public ICollection<Match> MatchTwo{ get; set; }
        public ICollection<Like> LikeRequester { get; set; }
        public ICollection<Like> LikeRequestee { get; set; }
        public ICollection<GatheringParticipant> Participations { get; set; }
    }
}
