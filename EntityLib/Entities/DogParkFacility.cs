using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityLib.Entities
{
    public class DogParkFacility
    {
        public enum DogPackFacilityType
        {
            Lake = 0,
            Grassfield = 1,
            Agility = 2,
            Fenced = 3
        }

        public int DogParkId { get; set; }
        public DogPackFacilityType FacilityType { get; set; }
    }
}