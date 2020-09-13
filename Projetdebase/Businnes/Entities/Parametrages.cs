using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("parametrages")]
    public class Parametrages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        [Column("id")]
        public int Id { get; set; }

        [Column("contenu")]
        public string Contenu { get; set; }

        [Column("type")]
        public int Type { get; set; }
    }
}
