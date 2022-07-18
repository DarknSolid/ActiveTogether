using EntityLib.Entities;
using ModelLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs.Reviews
{
    public class ReviewCreatedDTO
    {
        public int PlaceId { get; set; }
        public int UserId { get; set; }
    }
}
