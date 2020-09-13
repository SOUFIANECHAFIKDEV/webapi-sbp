using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{

    [Serializable]
    [Table("rubrique")]
    public class Rubrique
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [Column("nom")]
        public string Nom { get; set; }

        public virtual List<DocumentAttacher> DocumentAttacher { get; set; }
        
    }
}
