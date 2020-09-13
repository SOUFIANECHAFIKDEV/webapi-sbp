using ProjetBase.Businnes.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Entities
{

    [Table("fiche_intervention")]
    public partial class FicheIntervention
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("reference")]
        public string Reference { get; set; }

        [Column("statut")]
        public StatutFicheIntervention Status { get; set; }


        [Column("prestations")]
        public string Prestations { get; set; }

        [Column("site_intervention")]
        public string AdresseIntervention { get; set; }

        [Column("date_creation")]
        public DateTime DateCreation { get; set; }


        [Column("date_debut")]
        public DateTime DateDebut { get; set; }


        [Column("date_fin")]
        public DateTime DateFin { get; set; }

        [Column("rapport")]
        public string Rapport { get; set; }
  

        [Column("nbr_deplacement")]
        public int NombreDeplacement { get; set; }

        [Column("nbr_panier")]
        public int NombrePanier { get; set; }

        [Column("memos")]
        public string Memos { get; set; }

        [Column("historique")]
        public string Historique { get; set; }

        [Column("signature_client")]
        public string SignatureClient { get; set; }


        [Column("signature_technicien")]
        public string SignatureTechnicien { get; set; }

        [Column("emails")]
        public string Emails { get; set; }

        [Column("objet")]
        public string Objet { get; set; }
     
        [Column("id_chantier")]
        [ForeignKey("Chantier")]
        public int? IdChantier { get; set; }


        [Column("id_agenda_google")]
        public string idAgendaGoogle { get; set; }

        [Column("id_facture")]
        [ForeignKey("Facture")]
        public int? IdFacture { get; set; }

        public Chantier Chantier { get; set; }

        public virtual ICollection<InterventionTechnicien> InterventionTechnicien { get; set; }

        public virtual Facture Facture { get; set; }
    }
}
