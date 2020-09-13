using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models.Filter
{
    public class FicheTravailFilterModel
    {

        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public int? Statut { get; set; }
        public int? TechnicienId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateDebut { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }
        public int? FranchiseId { get; set; }
        public bool All { get; set; }

    }
}
