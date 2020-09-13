using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;

namespace ProjetBase.Businnes.Models.Filter
{
    public class DocAttacherFilterModel
    {
        public string SearchQuery;
        public DateTime DateDebut;
        public DateTime DateFin;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public int? Rubrique { get; set; }
        public List<string> Labels = new List<string>();
        public int? IdChantier;
    }
}
