using InoAuthentification.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("produit")]
    public class Produit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [MinLength(2)]
        [Column("nom")]
        public string Nom { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("designation")]
        public string Designation { get; set; }

        [Column("nomber_heure")]
        public int Nomber_heure { get; set; }

        [Column("cout_materiel")]
        public double Cout_materiel { get; set; }

        [Column("cout_vente")]
        public double Cout_vente { get; set; }

        [Column("prixHt")]
        public double PrixHt { get; set; }
        
        [Column("tva")]
        public double Tva { get; set; }

        [Column("fichesTechniques")]
        public string FichesTechniques { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [Column("unite")]
        public string Unite { get; set; }

        [Column("categorie")]
        public string Categorie { get; set; }

        [Column("labels")]
        public string Labels { get; set; }

        [NotMapped]
        public int Lot { get; set; }

        //[Column("id_fournisseur")]
        //[ForeignKey("Fournisseur")]
        //public int Id_fournisseur { get; set; }
        //public virtual Fournisseur Fournisseur { get; set; }


        //[Column("prix_fournisseur")]
        //public double Prix_fournisseur { get; set; }

        [NotMapped]
        public double Cout_horaire { get; set; }

        public Produit()
        {
            this.Cout_horaire = (this.Cout_horaire == null ? this.Cout_horaire : 0) * (this.Cout_vente == null ? this.Cout_vente : 0);
        }

        public virtual List<ProduitFournisseur> PrixParFournisseur { get; set; }
        public virtual ICollection<LotProduits> LotProduits { get; set; }
    }
}
