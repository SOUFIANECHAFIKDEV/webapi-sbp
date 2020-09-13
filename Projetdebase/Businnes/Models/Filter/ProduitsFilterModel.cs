using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;

namespace ProjetBase.Businnes.Models.Filter
{
    public class ProduitsFilterModel
    {
        public string SearchQuery;
        public List<string> Labels = new List<string>();
        public DateTime DateDebut;
        public DateTime DateFin;
        public int? IdFournisseur { get; set; }
        public string Categorie { get; set; }


        public PagingParams PagingParams;
        public SortingParams SortingParams;
    }
}
