using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLib.Entities
{
    public class DogBreed : SimplePrimaryKey
    {

        [Required]
        [MinLength(1)]
        public string Name { get; set; }
    }
}
