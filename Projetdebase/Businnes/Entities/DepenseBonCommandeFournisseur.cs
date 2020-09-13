using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Table("depense_boncommandefournisseur")]
    public class DepenseBonCommandeFournisseur
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_depense")]
        [ForeignKey("Depense")]
        public int? IdDepense { get; set; }

        [Column("id_boncommandefourniseur")]
        [ForeignKey("BonCommandeFournisseur")]
        public int? IdBonCommandeFournisseur { get; set; }


        public Depense Depense { get; set; }
        public BonCommandeFournisseur BonCommandeFournisseur { get; set; }

    }
}
