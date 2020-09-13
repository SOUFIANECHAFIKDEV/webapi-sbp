using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AppTestAuthentificationExample.Entities
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

        [Required]
        [MinLength(5)]
        [Column("reference")]
        public string Reference { get; set; }

        [Required]
        [MinLength(5)]
        [Column("raison_sociale")]
        public string RaisonSociale { get; set; }

        [Required]
        [MinLength(10)]
        [Column("telephone")]
        public string Telephone { get; set; }

        [Column("fax")]
        public string Fax { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Url]
        [Column("site_web")]
        public string SiteWeb { get; set; }

        [Required]
        [MinLength(10)]
        [Column("siret")]
        public string TvaIntraCommunautaire { get; set; }

        [Required]
        [MinLength(10)]
        [Column("tva_intra_communautaire")]
        public string CodeComptable { get; set; }

        [Required]
        [Column("typeClient")]
        public int TypeClient { get; set; }

        [Column("adresses")]
        public string Adresses { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("contacts")]
        public string Contacts { get; set; }


    }
}
