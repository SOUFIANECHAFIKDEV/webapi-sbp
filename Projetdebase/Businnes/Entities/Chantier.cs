using ProjetBase.Businnes.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{
    [Serializable]
    [Table("chantiers")]
    public class Chantier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nom")]
        public string Nom { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("commentaire")]
        public string Commentaire { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [Column("statut")]
        public StatutChantier Statut { get; set; }

        [Column("date_creation")]
        public DateTime? Date_creation { get; set; }

        [Column("documentation")]
        public string Documentation { get; set; }

        //[Column("adresse_travaux")]
        //public string AdresseTravaux { get; set; }
        [Column("nombre_heure")]
        public double NombrHeure { get; set; }

        [Column("montant")]
        public double Montant { get; set; }

        [Column("taux-avancement")]
        public int TauxAvancement { get; set; }

        [Column("id_client")]
        [ForeignKey("Client")]
        public int? IdClient { get; set; }

        public Client Client { get; set; }

        public IEnumerable<Devis> Devis { get; set; }
        public IEnumerable<BonCommandeFournisseur> BonCommandeFournisseur { get; set; }
        public IEnumerable<Facture> Factures { get; set; }
        public IEnumerable<Depense> Depense { get; set; }
        public IEnumerable<FicheIntervention> FicheIntervention { get; set; }

    }
}
