using EntityLib.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EntityLib.Entities.Enums;

namespace EntityLib.Entities
{
    public class Dog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [ForeignKey(nameof(DogBreed))]
        public int DogBreedId { get; set; }
        public DogBreed DogBreed { get; set; }

        [Required]
        public DogWeightClass WeightClass { get; set; }

        [Required]
        public DateTime Birth { get; set; }

        [Required]
        public bool IsGenderMale { get; set; }
    }
}
