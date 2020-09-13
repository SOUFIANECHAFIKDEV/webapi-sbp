﻿using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class VisiteMaintenanceModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
       // public List<int> Statut { get; set; }
        public int? Statut { get; set; }
        public int? Mois { get; set; }
        public int? Annee { get; set; }
        public int? IdClient { get; set; }
       // public int? IdContrat { get; set; }
    }
}
