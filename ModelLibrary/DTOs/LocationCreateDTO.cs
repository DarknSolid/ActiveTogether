using EntityLib.Entities.Constants;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs
{
    public class LocationCreateDTO
    {
        [Required]
        [StringLength(PlaceConstants.NameMaxLength, ErrorMessage = "Max 100 characters")]
        public string Name { get; set; }

        [Required]
        [StringLength(PlaceConstants.DescriptionMaxLength, ErrorMessage = "Max 5000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a location")]
        public NpgsqlPoint Point { get; set; }
    }
}
