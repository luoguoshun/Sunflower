﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OHEXML.Contracts.UserModule
{
    public class UserDTO
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
    }

    public class ResetPasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string RePassword { get; set; }
    }
}
