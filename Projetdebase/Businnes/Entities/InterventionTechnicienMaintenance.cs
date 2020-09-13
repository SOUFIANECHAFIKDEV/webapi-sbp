using InoAuthentification.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("intervention_technicien_maintenance")]
    public class InterventionTechnicienMaintenance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_technicien")]
        [ForeignKey("IdTechnicienNavigation")]
        public int IdTechnicien { get; set; }

        [Column("id_interventionmaintenance")]
        [ForeignKey("IdFicheInterventionMaintenanceNavigation")]
        public int IdFicheIntervention { get; set; }

        public virtual User IdTechnicienNavigation { get; set; }
        public virtual FicheInterventionMaintenance IdFicheInterventionMaintenanceNavigation { get; set; }
    }
}
