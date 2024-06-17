using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs.Instructors
{
    public class DogTrainingUpdateDTO : DogTrainingCreateDTO
    {
        public int DogTrainingId { get; set; }
    }
}
