using InoAuthentification.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("categorie")]
    public class Categorie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_unique")]
        public int Id { get; set; }

        [Required]
        [Column("Nom")]
        public string Nom { get; set; }

        [Required]
        [Column("Description")]
        public string Description { get; set; }

        [Column("Code_comptable")]
        public string Code_comptable { get; set; }
    }
}
