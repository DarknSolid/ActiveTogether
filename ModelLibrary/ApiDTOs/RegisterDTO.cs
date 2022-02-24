﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.ApiDTOs
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        public string RepeatedEmail { get; set; }
    }
}
