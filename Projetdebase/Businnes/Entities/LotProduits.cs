using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("lots_produits")]
    public class LotProduits
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }


        [Column("qte")]
        public int Qte { get; set; }

        [Column("idLot")]
        [ForeignKey("IdLotNavigation")]
        public int IdLot { get; set; }

        [Column("idProduit")]
        [ForeignKey("IdProduitNavigation")]
        public int IdProduit { get; set; }

        public virtual Lots IdLotNavigation { get; set; }
        public virtual Produit IdProduitNavigation { get; set; }
    }
}
