using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("departement")]
    public partial class Departement
    {

        public Departement()
        {
            Ville = new HashSet<Ville>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("departement_code")]
        public string DepartementCode { get; set; }

        [Column("departement_nom")]
        public string DepartementNom { get; set; }

        [Column("id_pays")]
        public int? IdPays { get; set; }

        public ICollection<Ville> Ville { get; set; }
    }
}
