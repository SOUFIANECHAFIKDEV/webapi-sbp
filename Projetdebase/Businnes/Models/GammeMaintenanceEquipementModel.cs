using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class GammeMaintenanceEquipementModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        
    }

    public class Site
    {
        public string designation { get; set; }
        public string adresse { get; set; }
        public string complementAdresse { get; set; }
        public string departement { get; set; }
        public string ville { get; set; }
        public string codePostal { get; set; }
        public Pays pays { get; set; }
        public bool @default { get; set; }
    }
}
