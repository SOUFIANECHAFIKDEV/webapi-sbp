using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class PrestationHistoriqueModel
    {
        public string Reference { get; set; }
        public string Nom { get; set; }
        public double Prix { get; set; }
        public double Tva { get; set; }
        public string Unite { get; set; }
        public string Categorie { get; set; }
    }
}
