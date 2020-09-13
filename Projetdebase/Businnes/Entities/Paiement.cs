using InoAuthentification.Entities;
using ProjetBase.Businnes.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Table("paiement")]
    public partial class Paiement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("comptabilise")]
        public int Comptabilise { get; set; }

        [Column("type")]
        public int type { get; set; }

        [Column("montant")]
        public double Montant { get; set; }

        [Column("id_caisse")]
        [ForeignKey("ParametrageCompte")]
        public int? IdCaisse { get; set; }

        [Column("date_paiement")]
        public DateTime DatePaiement { get; set; }

        [Column("id_mode_paiement")]
        [ForeignKey("ModeReglement")]
        public int? IdModePaiement { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("historique")]
        public string Historique { get; set; }


        [Column("id_avoir")]
        [ForeignKey("Avoir")]
        public int? IdAvoir { get; set; }

        [Column("id_depense")]
        [ForeignKey("Depense")]
        public int? IdDepense{ get; set; }

        [Column("update_at")]
        public long UpdateAt { get; set; } = EntityExtensions.UnixTimestampFromDateTime(DateTime.Now);


        public ModeReglement ModeReglement { get; set; }
        public ParametrageCompte ParametrageCompte { get; set; }
        public virtual ICollection<FacturePaiement> FacturePaiements { get; set; }
        public Avoir Avoir { get; set; }
        public Depense Depense { get; set; }
    }
}
