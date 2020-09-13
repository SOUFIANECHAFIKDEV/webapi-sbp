using InoAuthentification.Entities;
using ProjetBase.Businnes.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Table("avoir")]
    public class Avoir
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("date_creation")]
        public DateTime DateCreation { get; set; }

        [Column("date_echeance")]
        public DateTime DateEcheance { get; set; }

        [Column("prestations")]
        public string Prestations { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("remise")]
        public double Remise { get; set; }

        [Column("type_remise")]
        public string TypeRemise { get; set; }

        [Column("total")]
        public double Total { get; set; }

        [Column("tva")]
        public string Tva { get; set; }

        [Column("puc")]
        public double PUC { get; set; }


        [Column("tvaGlobal")]
        public string TvaGlobal { get; set; }

        [Column("prorata")]
        public double Prorata { get; set; }

        [Column("retenue_garantie")]
        public double? RetenueGarantie { get; set; }

        [Column("delai_garantie")]
        public int? DelaiGarantie { get; set; }

        [NotMapped]
        public DateTime DateEcheanceGarantie { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("comptabilise")]
        public int Comptabilise { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("condition_regelement")]
        public string ConditionRegelement { get; set; }

        [Column("object")]
        public string Object { get; set; }

        [Column("info_client")]
        public string InfoClient { get; set; }

        [Column("id_facture")]
        [ForeignKey("Facture")]
        public int? IdFacture { get; set; }

        [Column("total_ht")]
        public double TotalHt { get; set; }

     

        [Column("compteur")]
        public int Compteur { get; set; }

        [Column("update_at")]
        public long UpdateAt { get; set; } = EntityExtensions.UnixTimestampFromDateTime(DateTime.Now);

     

        [Column("id_chantier")]
        [ForeignKey("Chantier")]
        public int? IdChantier { get; set; }


        [Column("id_client")]
        //[ForeignKey("Client")]
        public int? IdClient { get; set; }

        public Chantier Chantier { get; set; }
        public Facture Facture { get; set; }
        public virtual ICollection<Paiement> Paiements { get; set; }
        [NotMapped]
        public Client Client { get; set; }
    }
}
