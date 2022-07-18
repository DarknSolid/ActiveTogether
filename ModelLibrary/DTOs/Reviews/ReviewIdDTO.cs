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
        public int RevieweeId { get; set; }
        public int ReviewerId { get; set; }
    }
}
