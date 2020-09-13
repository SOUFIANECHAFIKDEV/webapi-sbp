using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class GammeMaintenanceVisiteModel
    {
        public int id { get; set; }
        public  string nom { get; set; }
        public List<LibelleModel> libelle { get; set; }

    }
    public class LibelleModel
    {
        public int id { get; set; }
        public string nom { get; set; }
        public List<OperationsModel> operations { get; set; }

    }
    public class OperationsModel
    {
        public int id { get; set; }
        public string nom { get; set; }

    }

}
