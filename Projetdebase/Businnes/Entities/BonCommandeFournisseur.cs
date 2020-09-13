using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Table("bon_commande_fournisseur")]
    public partial class BonCommandeFournisseur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("idChantier")]
        [ForeignKey("Chantier")]
        public int? IdChantier { get; set; }

        [Column("status")]
        public int Status { get; set; } 

        [Column("articles")]
        public string Articles { get; set; }

        [Column("date_creation")]
        public DateTime DateCreation { get; set; }

        [Column("date_expiration")]
        public DateTime DateExpiration { get; set; }
        

        [Column("total")]
        public double Total { get; set; }


        [Column("tva")]
        public string Tva { get; set; }
        

        [Column("total_ht")]
        public double TotalHt { get; set; }

        [Column("objet")]
        public string Objet { get; set; }

        [Column("note")]
        public string note { get; set; }

        [Column("conditions_reglement")]
        public string ConditionsReglement { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("historique")]
        public string Historique { get; set; }



        [Column("id_fournisseur")]
        [ForeignKey("Fournisseur")]
        public int? IdFournisseur { get; set; }


        [Column("id_devis")]
        [ForeignKey("Devis")]
        public int? IdDevis { get; set; }

        public Devis Devis { get; set; }

        public Chantier Chantier { get; set; }
        public Fournisseur Fournisseur { get; set; }

        public virtual ICollection<DepenseBonCommandeFournisseur> DepenseBonCommandeFournisseurs { get; set; }

        //public virtual DepenseBonCommandeFournisseur DepenseBonCommandeFournisseurs { get; set; }
    }
}