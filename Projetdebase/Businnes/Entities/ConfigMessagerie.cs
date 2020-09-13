using InoAuthentification.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Table("config_messagerie")]
    public class ConfigMessagerie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("serveur")]
        public string Serveur { get; set; }

        [Column("port")]
        public int Port { get; set; }

        [Column("ssl")]
        public int Ssl { get; set; }

        [Column("id_societe")]
        public string IdSociete { get; set; }
    }
}
