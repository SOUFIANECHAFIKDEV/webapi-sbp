using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace AppTestAuthentificationExample.Entities
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
        [Column("nom")]
        public string Nom { get; set; }

        [Required]
        [Column("raison_sociale")]
        public string RaisonSociale { get; set; }

        [Required]
        [Column("id_pays")]
        [ForeignKey("Pays")]
        public int IdPays { get; set; }

        [Required,MinLength(8)]
        [Column("adresse")]
        public string Adresse { get; set; }

        [Column("complement_adresse")]
        public string ComplementAdresse { get; set; }

        [Column("id_ville")]
        [ForeignKey("Ville")]
        public int? IdVille { get; set; }

        [Column("autre_ville")]
        public string AutreVille { get; set; }

        [Column("code_postal")]
        public string CodePostal { get; set; }

        [Required]
        [Column("id_departement")]
        [ForeignKey("Departement")]
        public int IdDepartement { get; set; }

        [Column("telephone")]
        public string Telephone { get; set; }

        [Column("fax")]
        public string Fax { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("site_web")]
        public string SiteWeb { get; set; }

        [Column("siret")]
        public string TvaIntraCommunautaire { get; set; }

        [Column("tva_intra_communautaire")]
        public string CodeComptable { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("contacts")]
        public string Contacts { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        public virtual Pays Pays { get; set; }
        public virtual Ville Ville { get; set; }
        public virtual Departement Departement { get; set; }
    }
}
