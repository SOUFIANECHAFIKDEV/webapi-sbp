using InoAuthentification.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("client")]
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [Column("nom")]
        public string Nom { get; set; }

       
        [Column("reference")]
        public string Reference { get; set; }


        [Required]
        [Column("codeclient")]
        public string Codeclient { get; set; }


        [MinLength(10)]
        [Column("telephone")]
        public string Telephone { get; set; }

        [Column("fax")]
        public string Fax { get; set; }


        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Column("email")]
        public string Email { get; set; }
        
        [Column("site_web")]
        public string SiteWeb { get; set; }

        [Column("siret")]
        public string Siret { get; set; }

        [Column("tva_intra_communautaire")]
        public string TvaIntraCommunautaire { get; set; }

        [Column("adresses")]
        public string Adresses { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("contacts")]
        public string Contacts { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [Column("code_comptable")]
        public string CodeComptable { get; set; }

        [Column("id_agent")]
        [ForeignKey("User")]
        public int? IdAgent { get; set; }
        public User User { get; set; }

        [Column("id_groupe")]
        [ForeignKey("Groupe")]
        public int? IdGroupe { get; set; }
        public Groupe Groupe { get; set; }
    }
}
