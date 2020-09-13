using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models
{
    public class FicheInterventionMaintenanceModel
    {
        public string SearchQuery;
        public PagingParams PagingParams;
        public SortingParams SortingParams;
        // public StatutDevis statut { get; set; }
        public int? Statut { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateDebut { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }
        public int? IdVisiteIntervention { get; set; }
        public bool All { get; set; }
        public int? IdClient { get; set; }
        public int? idTechnicien { get; set; }
    }
}
