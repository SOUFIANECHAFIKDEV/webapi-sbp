using InoAuthentification.Entities;
using ProjetBase.Businnes.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Table("parametrage_compte")]
    public class ParametrageCompte
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("code_comptable")]
        public string code_comptable { get; set; }

        [Column("type")]
        public int Type { get; set; }

      
    }
}
