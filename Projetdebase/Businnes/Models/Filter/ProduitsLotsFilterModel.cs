using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;

namespace ProjetBase.Businnes.Models.Filter
{
    public class ProduitsLotsFilterModel
    {
        public string SearchQuery;
        public List<string> Labels = new List<string>();
        public DateTime DateDebut;
        public DateTime DateFin;
        public List<int> produits = new List<int>();
        public PagingParams PagingParams;
        public SortingParams SortingParams;
    }
}
