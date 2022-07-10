using EntityLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.DogParkFacility;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.DogPark
{
    public class DogParkDetailedDTO : DogParkListDTO
    {
        public string Description { get; set; }
        public double Rating { get; set; }
        public IEnumerable<DogPackFacilityType> Facilities { get; set; }
        public List<string> ImageUrls { get; set; }
        public int TotalReviews { get; set; }

        public DogParkDetailedDTO()
        {
            ImageUrls = new List<string>();
            Facilities = new List<DogPackFacilityType>();
        }

        public ReviewStatus CurrentReviewStatus { get; set; }
        public enum ReviewStatus
        {
            MustCheckIn,
            CanUpdateReview,
            CanReview

        }
    }
}
