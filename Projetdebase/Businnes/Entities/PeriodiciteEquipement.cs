using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("periode")]
    public class PeriodiciteEquipement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("mois")]
        public int Mois { get; set; }

        [Column("statut")]
        public int statut { get; set; }

        [Column("observation")]
        public string Observation { get; set; }

        [Column("id_operatione_equipement")]
        [ForeignKey("OperationsEquipement")]
        public int idOperationsEquipement { get; set; }

        public OperationsEquipement OperationsEquipement { get; set; }
    }

}
