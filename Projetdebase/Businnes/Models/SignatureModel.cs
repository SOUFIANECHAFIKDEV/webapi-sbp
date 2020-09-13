using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class SignatureModel
    {
        public DateTime date { get; set; }
        public string nom { get; set; }
        public int? idTechnicien { get; set; }
        public string signature { get; set; }
    }
}
