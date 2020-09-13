using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InoAuthentification.Entities
{
    [Table("user")]
    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("nom")]
        public string Nom { get; set; }
        [Column("prenom")]
        public string Prenom { get; set; }


        [Column("actif")]
        public int? Actif { get; set; }

        [Column("dernierecon")]
        public DateTime? Dernierecon { get; set; }

        [Column("join_date")]
        public DateTime JoinDate { get; set; }

        [Column("email")]
        public string Email { get; set; }
        [Column("passwordhash")]
        [JsonIgnore]
        public string Passwordhash { get; set; }
        [Column("phonenumber")]
        public string Phonenumber { get; set; }
        [Column("accessfailedcount")]
        public int Accessfailedcount { get; set; }
        [Column("username")]
        public string Username { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [Column("matricule")]
        public string Matricule { get; set; }

        [Column("idProfile")]
        public int IdProfile { get; set; }

        [Column("passwordAlreadyChanged")]
        public int PasswordAlreadyChanged { get; set; }

        [Column("is_deleted", TypeName = "bit")]
        public bool IsDeleted { get; set; } = false;

        [Column("idClient")]
        [ForeignKey("Client")]
        public int? IdClient { get; set; }
    }
}
