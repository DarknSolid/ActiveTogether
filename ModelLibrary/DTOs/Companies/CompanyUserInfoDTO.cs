using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace ModelLib.DTOs.Companies
{
    public class CompanyUserInfoDTO
    {
        public FacilityType CompanyType { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyProfilePictureUrl { get; set; }
    }
}
