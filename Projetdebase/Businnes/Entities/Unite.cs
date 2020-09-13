using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("unite")]
    public class Unite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nomcomplet")]
        public string NomComplet { get; set; }

        [Column("abreviation")]
        public string Abreviation { get; set; }
    }
}
