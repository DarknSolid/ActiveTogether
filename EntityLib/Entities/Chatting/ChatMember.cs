using EntityLib.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLib.Entities.Chatting
{
    public class ChatMember
    {
        [ForeignKey(nameof(Chat))]
        public int ChatId { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Message))]
        public int? LastReadId { get; set; }

        public ApplicationUser User { get; set; }
        public Chat Chat { get; set; }
        public Message LastRead { get; set; }
    }
}
