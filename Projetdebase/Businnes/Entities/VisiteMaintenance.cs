using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
 
    [Table("visite_maintenance")]
    public class VisiteMaintenance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
       public int id { get; set; }

        [Column("statut")]
        public int Statut { get; set; }

        [Column("mois")]
        public int Mois { get; set; }

        [Column("annee")]
        public int Annee { get; set; }

        [Column("gamme_maintenance")]
        public string GammeMaintenance { get; set; }

        [Column("id_contrat")]
        [ForeignKey("ContratEntretien")]
        public int IdContratEntretien { get; set; }

        public ContratEntretien ContratEntretien { get; set; }
        public IEnumerable<FicheInterventionMaintenance> FicheInterventionMaintenance { get; set; }


    }
}
