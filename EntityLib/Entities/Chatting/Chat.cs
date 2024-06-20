using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Matching;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EntityLib.Entities.Chatting
{
    public class Chat : DateAndIntegerId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateTime { get; set; }

        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }

        public Place Place { get; set; }
        public ICollection<ChatMember> ChatMembers { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
