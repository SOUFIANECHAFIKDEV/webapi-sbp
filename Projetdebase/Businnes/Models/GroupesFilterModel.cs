using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class GroupesFilterModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
    }
}
