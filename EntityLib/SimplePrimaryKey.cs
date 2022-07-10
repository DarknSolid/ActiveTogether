using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLib
{
    public class SimplePrimaryKey
    {
        [Key]
        public int Id { get; set; }
    }
}
