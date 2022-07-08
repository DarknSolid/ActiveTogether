using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.ApiDTOs.DogParks
{
    public class DogParkReviewsDTO
    {
        public PaginationRequest PaginationRequest { get; set; } = new();
        public int DogParkId { get; set; }
    }
}
