using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models.Filter
{
    public class DepenseFilterModel
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
    public class DepensePostModel
    {
        public Entities.Depense Depense { get; set; }

        public List<int> BonCommandeFournisseurIds { get; set; }

    }
}
