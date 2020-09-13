using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class ConfigMessagerieModel
    {
       
            public string server { get; set; }
            public int port { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public int ssl { get; set; }
        
    }
}
