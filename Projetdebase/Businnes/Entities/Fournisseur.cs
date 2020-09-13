using InoAuthentification.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("fournisseur")]
    public class Fournisseur
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("reference")]
        public string Reference { get; set; }

        [Required]
        [MinLength(5)]
        [Column("nom")]
        public string Nom { get; set; }

       

        [Required]
        [Column("id_pays")]
        [ForeignKey("Pays")]
        public int IdPays { get; set; }

        [Required]
        [Column("adresse")]
        public string Adresse { get; set; }

        [Column("complement_adresse")]
        public string ComplementAdresse { get; set; }

        [Column("ville")]
        public string Ville { get; set; }

        [Column("code_postal")]
        public string CodePostal { get; set; }

        [Column("departement")]
        public string Departement { get; set; }

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

        [Column("code_comptable")]
        public string CodeComptable { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("contacts")]
        public string Contacts { get; set; }

        [Column("historique")]
        public string Historique { get; set; }
        

        public Pays Pays { get; set; }
    }
}
