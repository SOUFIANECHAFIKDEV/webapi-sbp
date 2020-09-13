using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models.Filter
{
    public class BonCommandeFournisseurFilterModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int? IdClient { get; set; }
        public int? IdChantier { get; set; }
        public int? IdFournisseur { get; set; }
        public int? Statut { get; set; }
    }

    public class BonCommandeFournisseurModel
    {
        public double validite_boncommande { get; set; }
        public string objet_boncommande { get; set; }
        public string note_boncommande { get; set; }
        public string conditions_boncommande { get; set; }
    }
}
