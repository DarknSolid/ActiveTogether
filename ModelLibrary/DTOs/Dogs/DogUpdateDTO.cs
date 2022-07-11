
using static EntityLib.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ModelLib.DTOs.Dogs
{
    public class DogUpdateDTO : DogCreateDTO
    {
        public int Id { get; set; }
    }
}
