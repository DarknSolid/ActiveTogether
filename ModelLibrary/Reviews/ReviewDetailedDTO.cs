using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Reviews
{
    public class ReviewDetailedDTO : ReviewCreateDTO
    {
        public string AuthorName { get; set; }
        public DateTime Date { get; set; }


    }
}
