using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class AvoirAllModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        public List<int> Statut { get; set; }
        //public int? Statut { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int? IdChantier { get; set; }
        public int? IdClient { get; set; }
        public double? MaxTotal { get; set; }

    }

    public class AvoirModel
    {
        public int validite_avoir { get; set; }
        public string objet_avoir { get; set; }
        public string conditions_avoir { get; set; }
        public string entete_avoir { get; set; }
        //public string piedavoir { get; set; }
        //public string conditionReglementAvoir { get; set; }
    }

    public class Pays
    {
        public int id { get; set; }
        public int code { get; set; }
        public string nomEnGb { get; set; }
        public string nomFrFr { get; set; }
    }

    public class AdresseFacturation
    {
        public string designation { get; set; }
        public string adresse { get; set; }
        public string complementAdresse { get; set; }
        public string departement { get; set; }
        public string ville { get; set; }
        public string codePostal { get; set; }
        public Pays pays { get; set; }
        public bool @default { get; set; }
    }

    public class InfoClientModel
    {
        public string codeClient { get; set; }
        public string nom { get; set; }
        public AdresseFacturation adresseFacturation { get; set; }
    }
}
