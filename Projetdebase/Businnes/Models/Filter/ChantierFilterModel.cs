using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models.Filter
{
    public class ChantierFilterModel
    {
        public string SearchQuery { get; set; }
        public StatutChantier? statut { get; set; }
        public PagingParams PagingParams { get; set; }
        public SortingParams SortingParams { get; set; }
        public int? ClientId { get; set; }
    }

    public class Documentation {
        public int id { get; set; }
        public string docs { get; set; }
        public string addedFile { get; set; }
        public string removedFile { get; set; }
    }
}
