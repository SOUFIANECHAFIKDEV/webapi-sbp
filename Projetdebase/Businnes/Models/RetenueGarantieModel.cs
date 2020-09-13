using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class RetenueGarantieModel
    {
        public int idFacture;
        public string reference;
        public double? valeurRetenue;
        public double? htRetenus;
        public DateTime? dateEcheanceRetenue;
        public int? statutRetenue;

    }
}
