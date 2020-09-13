using System;
using System.Collections.Generic;
using System.Text;

namespace InoAuthentification.JwtManagers.Models
{
    public class TokenModel
    {
       public long Expire { get; set; }
       public int UserId { get; set; }
    }
}
