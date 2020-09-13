using System;
using System.Collections.Generic;
using System.Text;

namespace InoAuthentification.UserManager.Models
{
    public class TokenModel
    {
        public string Token { get; set; } 
        public UserModel User { get; set; } 
    }
}
