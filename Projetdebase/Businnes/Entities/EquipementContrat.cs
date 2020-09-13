using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("equipement_contrat")]
    public class EquipementContrat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Column("nom")]
        public string Nom { get; set; }

        [Column("id_contrat")]
        [ForeignKey("ContratEntretien")]
        public int idContrat { get; set; }

        public ContratEntretien ContratEntretien { get; set; }

        public IEnumerable<LibelleEquipement> Libelle { get; set; }
    }
}
