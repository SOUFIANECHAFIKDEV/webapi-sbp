using System;
using System.Collections.Generic;
using System.Text;

namespace InoAuthentification.JwtManagers.Models
{
    public class TokenValidation
    {
        public bool IsValid { get; set; }
        public TokenModel CurrentToken { get; set; }
    }
}
