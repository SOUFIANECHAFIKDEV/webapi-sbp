using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class JournalVenteFilter
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public int Periode { get; set; }
        public DateTime? DateMinimal { get; set; }
        public DateTime? DateMaximale { get; set; }
    }
    public class JournalVenteModel
    {
        public string CodeJournal { get; set; }
        public DateTime DateCreation { get; set; }
        public string NumeroCompte { get; set; }
        public string NumeroPiece { get; set; }
        public string NomClient { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; } 
    }

    public class JournalVenteSelectModel
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public DateTime DateCreation { get; set; }
        public string NomClient { get; set; }
        public string CodeComptableClient { get; set;}
        public string Articles { get; set; }
        public double TotalTTC { get; set; }
        public int Type { get; set; }
        public double Remise { get; set; }
        public string TypeRemise { get; set; }
        public string TVA { get; set; }
        public double TotalHT { get; set; }
        public int? IdFournisseur { get; set; }
    }

    public class GroupeArticle
    {
        public double Total { get; set; }
        public string CodeComptable { get; set; }
    }

    public class PagedComptabiliteList<T>
    {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> List { get; set; }
    }

}
