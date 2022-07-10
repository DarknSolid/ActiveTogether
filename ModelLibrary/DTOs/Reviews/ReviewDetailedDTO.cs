using EntityLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs.Reviews
{
    public class ReviewDetailedDTO : ReviewCreateDTO
    {
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string AuthorName { get { return AuthorFirstName + " " + AuthorLastName; } internal set { } }
        public DateTime Date { get; set; }
        public Enums.ReviewType ReviewType { get; set; }
    }
}
