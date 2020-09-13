using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("produit_fournisseur")]
    public class ProduitFournisseur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int? id { get; set; }

        [Required]
        [Column("id_produit")]
        [ForeignKey("produit")]
        public int idProduit { get; set; }

        [Required]
        [Column("id_fournisseur")]
        [ForeignKey("fournisseur")]
        public int idFournisseur { get; set; }

        [Column("prix")]
        public double prix { get; set; }

        [Column("default")]
        public int @default { get; set; }

        public virtual Fournisseur fournisseur { get; set; }
        public virtual Produit produit { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Column("id")]
        //public int Id { get; set; }

        //[Required]
        //[Column("id_produit")]
        //[ForeignKey("Produit")]
        //public int IdProduit { get; set; }

        //[Required]
        //[Column("id_fournisseur")]
        //[ForeignKey("Fournisseur")]
        //public int IdFournisseur { get; set; }

        //[Column("prix")]
        //public double? Prix { get; set; }

        //[Column("default")]
        //public int Default { get; set; }

        //public virtual Fournisseur Fournisseur { get; set; }
        //public virtual Produit Produit { get; set; }
    }
}
