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
    [Table("intervention_technicien")]
    public class InterventionTechnicien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_technicien")]
        [ForeignKey("IdTechnicienNavigation")]
        public int IdTechnicien { get; set; }

        [Column("id_fiche_intervention")]
        [ForeignKey("IdFicheInterventionNavigation")]
        public int IdFicheIntervention { get; set; }

        public virtual User IdTechnicienNavigation { get; set; }
        public virtual FicheIntervention IdFicheInterventionNavigation { get; set; }

    }
}
