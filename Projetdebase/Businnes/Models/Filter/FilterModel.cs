using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;

namespace ProjetBase.Businnes.Models.Filter
{
    public class FilterModel
    {
        public string SearchQuery;
        public DateTime DateDebut;
        public DateTime DateFin;

        public PagingParams PagingParams;
        public SortingParams SortingParams;
    }
}
