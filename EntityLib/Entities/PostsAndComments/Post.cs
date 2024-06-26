﻿using EntityLib.Entities.AbstractClasses;
using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities.PostsAndComments
{
    public class Post : DateAndIntegerId
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateTime { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Place))]
        public int? PlaceId { get; set; }

        public string? Body { get; set; }

        public string[]? MediaUrls { get; set; }

        public PostArea? Area { get; set; }
        public Interests? Category { get; set; }

        public ICollection<Reaction>? Reactions { get; set; }
        public ICollection<PostComment>? PostComments { get; set; }

        public ApplicationUser User { get; set; }
        public Place? Place { get; set; }
    }
}
