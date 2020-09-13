using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("labels")]
    public class Labels : IEquatable<Labels>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [Column("label")]
        public string Label { get; set; }

        [NotMapped]
        public int NobmbreDEFois { get; internal set; }

        public bool Equals(Labels other)
        {
            return Label.Equals(other.Label);
        }
    }
}
