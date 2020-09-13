using ProjetBase.Businnes.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Table("devis")]
    public partial class Devis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("total")]
        public double Total { get; set; }

        [Column("status")]
        public StatutDevis Status { get; set; }

        [Column("objet")]
        public string Objet { get; set; }

        [Column("condition_reglement")]
        public string ConditionReglement { get; set; }

        [Column("prestations")]
        public string Prestation { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [Column("remise")]
        public double Remise { get; set; }

        [Column("retenue_garantie")]
        public double? RetenueGarantie { get; set; }

        [Column("delai_garantie")]
        public int? DelaiGarantie { get; set; }

        [NotMapped]
        public DateTime DateEcheanceGarantie { get; set; }

        [Column("type_remise")]
        public string TypeRemise { get; set; }

        [Column("puc")]
        public double PUC { get; set; }

        [Column("tva")]
        public string Tva { get; set; }

        [Column("tvaGlobal")]
        public string TvaGlobal { get; set; }
        
        [Column("prorata")]
        public double Prorata { get; set; }

        [Column("total_ht")]
        public double TotalHt { get; set; }

        [Column("nomber_heure")]
        public double NomberHeure { get; set; }

        [Column("cout_vente")]
        public double CoutVente { get; set; }

        [Column("cout_materiel")]
        public double CoutMateriel { get; set; }

        [Column("achat_materiel")]
        public double AchatMateriel { get; set; }

        [Column("date_creation")]
        public DateTime DateCreation { get; set; }

        [Column("adresse_facturation")]
        public string AdresseFacturation { get; set; }

        [Column("adresse_intervention")]
        public string AdresseIntervention { get; set; }

        [Column("devis_exel")]
        public string DevisExel { get; set; }

        [Column("emails")]
        public string Emails { get; set; }

        [Column("situation")]
        public string Situation { get; set; }

        [Column("acomptes")]
        public string Acomptes { get; set; }
        
        [Column("id_chantiers")]
        [ForeignKey("Chantier")]
        public int? IdChantier { get; set; }

        public Chantier Chantier { get; set; }

        public virtual ICollection<BonCommandeFournisseur> BonCommandeFournisseur { get; set; }

        public virtual ICollection<Facture> Facture { get; set; }
    }

}
