using InoAuthentification.DbContexts;
using Microsoft.EntityFrameworkCore;
using InoAuthentification.Entities;
using ProjetBase.Businnes.Entities;

namespace ProjetBase.Businnes.Contexts
{
    public partial class ProjetBaseContext : InoAuthentificationDbContext
    {
        public ProjetBaseContext()
        {
        }

        public ProjetBaseContext(DbContextOptions<ProjetBaseContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Groupe> Groupes { get; set; }
		public virtual DbSet<Departement> Departement { get; set; }
		public virtual DbSet<Pays> Pays { get; set; }        
        public virtual DbSet<Ville> Ville { get; set; }
        public virtual DbSet<Parametrages> Parametrages { get; set; }
 		public virtual DbSet<Fournisseur> Fournisseurs { get; set; }
 		public virtual DbSet<Produit> Produit { get; set; }
 		public virtual DbSet<Categorie> Categorie { get; set; }
 		public virtual DbSet<Unite> Unite { get; set; }
		public virtual DbSet<Tva> Tva { get; set; }
        public virtual DbSet<Labels> Labels { get; set; }
        public virtual DbSet<Lots> Lots { get; set; }
        public virtual DbSet<Devis> Devis { get; set; }
        public virtual DbSet<Chantier> Chantier { get; set; }
 
        public virtual DbSet<LotProduits> LotProduits { get; set; }
        public virtual DbSet<FicheIntervention> FicheIntervention { get; set; }
        public virtual DbSet<InterventionTechnicien> InterventionTechnicien { get; set; }
        public virtual DbSet<Facture> Factures { get; set; }
        public virtual DbSet<Avoir> Avoirs { get; set; }
        public virtual DbSet<Rubrique> Rubrique { get; set; }
        public virtual DbSet<DocumentAttacher> DocumentAttacher { get; set; }
        public virtual DbSet<ParametrageCompte> ParametrageCompte { get; set; }
        public virtual DbSet<ModeReglement> ModeReglement { get; set; }
        public virtual DbSet<Paiement> Paiements { get; set; }
        public virtual DbSet<FacturePaiement> FacturePaiements { get; set; }
        public virtual DbSet<BonCommandeFournisseur> BonCommandeFournisseur { get; set; }
        public virtual DbSet<Depense> Depense { get; set; }
        public virtual DbSet<DepenseBonCommandeFournisseur> DepenseBonCommandeFournisseur { get; set;}
        public virtual DbSet<PeriodeComptable> PeriodeComptables { get; set; }
        public virtual DbSet<ProduitFournisseur> ProduitFournisseur { get; set; }
        public virtual DbSet<GammeMaintenanceEquipement> Equipements { get; set; }
        public virtual DbSet<ContratEntretien> ContratEntretien { get; set; }
        public virtual DbSet<EquipementContrat> EquipementContrat { get; set; }
        public virtual DbSet<LibelleEquipement> Libelle { get; set; }
        public virtual DbSet<OperationsEquipement> OperationsEquipement { get; set; }
        public virtual DbSet<PeriodiciteEquipement> Periodes { get; set; }
        public virtual DbSet<VisiteMaintenance> VisiteMaintenance { get; set; }
        public virtual DbSet<FicheInterventionMaintenance> FicheInterventionMaintenance { get; set; }
        public virtual DbSet<InterventionTechnicienMaintenance> InterventionTechnicienMaintenance { get; set; }
    }
}
