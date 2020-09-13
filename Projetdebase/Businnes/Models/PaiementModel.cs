using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class PaiementPostModel
    {
        public Paiement Paiement { get; set; }
        public int IdFacture { get; set; }
    }

    public class PaiementModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public int? IdCompte { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }

    public class PaiementMovementCompteACompteModel
    {
        public int CompteDebiter { get; set; }
        public int CompteCrediter { get; set; }
        public double Montant { get; set; }
        public DateTime datePaiement { get; set; }
        public int idModePaiement { get; set; }
        public string historique { get; set; }
    }

}