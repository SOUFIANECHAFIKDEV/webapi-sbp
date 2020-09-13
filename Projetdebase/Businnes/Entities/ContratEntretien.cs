using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("contrat_entretien")]
    public class ContratEntretien
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("statut")]
        public int Statut { get; set; }


        [Column("date_debut")]
        public DateTime DateDebut { get; set; }

        [Column("date_fin")]
        public DateTime DateFin { get; set; }

        [Column("site")]
        public string Site { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("pieces_jointes")]
        public string PiecesJointes { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [Column("id_client")]
        [ForeignKey("Client")]
        public int IdClient { get; set; }

        public Client Client { get; set; }

        public IEnumerable<EquipementContrat> EquipementContrat { get; set; }
        public IEnumerable<VisiteMaintenance> VisiteMaintenance { get; set; }

    }
}
