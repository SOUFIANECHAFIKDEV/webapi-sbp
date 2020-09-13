using ProjetBase.Businnes.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("mode_reglement")]
    public class ModeReglement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        public string Nom { get; set; }

        [Column("update_at")]
        public long UpdateAt { get; set; } = EntityExtensions.UnixTimestampFromDateTime(DateTime.Now);

        [Column("is_deleted", TypeName = "bit")]
        public bool IsDeleted { get; set; } = false;
    }
}
