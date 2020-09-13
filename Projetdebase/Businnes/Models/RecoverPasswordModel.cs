using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class RecoverPasswordModel
    {
        public string email { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
