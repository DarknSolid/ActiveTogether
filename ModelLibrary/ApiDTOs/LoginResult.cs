using ModelLib.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.ApiDTOs
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public bool MustConfirmEmail { get; set; }
        public UserDetailedDTO? UserDetailedInfo { get; set; }

    }
}
