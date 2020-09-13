using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class ComptabiliteComptesFilter
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public bool IsCaisse { get; set; }
        public int Periode { get; set; }
        public DateTime? DateMinimal { get; set; }
        public DateTime? DateMaximale { get; set; }
    }

    public class JournalBanqueModel
    {
        public string CodeJournal { get; set; }
        public DateTime DatePaiement { get; set; }
        public string NumeroCompte { get; set; }
        public string NumeroPiece { get; set; }
        public string Tiers { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string TypePaiement { get; set; }
    }
}
