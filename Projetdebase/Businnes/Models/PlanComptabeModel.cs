using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class PlanComptabeModel
    {
        public int id { get; set; }
        public string label { get; set; }
      //  public (int, List<PlanComptabeTvaModel>) codeComptable { get; set; }
        public int codeComptable { get; set; }
    }

    public class PlanComptabeTvaModel
    {
        public string tva { get; set; }
        public double codeComptable { get; set; }
    }
}
