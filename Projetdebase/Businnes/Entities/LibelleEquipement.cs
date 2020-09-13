using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("lot_equipement")]
    public class LibelleEquipement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("id_equipemntContrat")]
        [ForeignKey("EquipementContrat")]
        public int idEquipementContrat { get; set; }

        public EquipementContrat EquipementContrat { get; set; }
        public IEnumerable<OperationsEquipement> OperationsEquipement { get; set; }
    }
}
