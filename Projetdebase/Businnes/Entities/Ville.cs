using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("ville")]
    public partial class Ville
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_departement")]
        [ForeignKey("IdDepartementNavigation")]
        public int? IdDepartement { get; set; }

        [Column("ville_nom")]
        public string VilleNom { get; set; }

        [Column("ville_nom_reel")]
        public string VilleNomReel { get; set; }

        [Column("code_postal")]
        public string CodePostal { get; set; }

        public virtual Departement IdDepartementNavigation { get; set; }
    }
}
