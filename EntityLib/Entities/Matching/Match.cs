using EntityLib.Entities.AbstractClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib.Entities.Identity;
using EntityLib.Entities.Chatting;

namespace EntityLib.Entities.Matching
{
    public class Match : DateAndIntegerId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateTime { get; set; }

        [Required]
        public bool IsIndividualMatch { get; set; }

        [Required]
        public int TargetOneId { get; set; }
        [Required]
        public int TargetTwoId { get; set; }

        [Required]
        [ForeignKey(nameof(Chat))]
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public ApplicationUser TargetOne { get; set; }
        public ApplicationUser TargetTwo { get; set; }
    }
}
