using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("operations_equipement")]
    public class OperationsEquipement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("id_lotequipement")]
        [ForeignKey("LibelleEquipement")]
        public int idLotEquipement { get; set; }

        public LibelleEquipement LibelleEquipement { get; set; }
        public IEnumerable<PeriodiciteEquipement> Periodicite { get; set; }

    }
}
