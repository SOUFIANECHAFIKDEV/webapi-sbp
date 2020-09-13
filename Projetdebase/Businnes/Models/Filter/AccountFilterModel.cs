using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models.Filter
{
    public class AccountFilterModel
    {
        public string SearchQuery;
        public DateTime DateDebut;
        public DateTime DateFin;

        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public List<int> ProfileType;
        
    }
}
