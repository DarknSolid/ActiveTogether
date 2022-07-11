
using System.ComponentModel.DataAnnotations;

namespace ModelLib.DTOs.Dogs
{
    public class DogListDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Birth { get; set; }

        public bool IsGenderMale { get; set; }

        public int Breed { get; set; }
        public string BreedName { get; set; }

    }
}
