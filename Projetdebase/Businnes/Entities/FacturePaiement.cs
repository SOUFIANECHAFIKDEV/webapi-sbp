using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Table("facture_paiement")]
    public partial  class FacturePaiement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_facture")]
        [ForeignKey("Facture")]
        public int IdFacture { get; set; }

        [Column("id_paiement")]
        [ForeignKey("Paiement")]
        public int IdPaiement { get; set; }

        [Column("montant")]
        public double Montant { get; set; }

        public Facture Facture { get; set; }
        public Paiement Paiement { get; set; }
    }
}
