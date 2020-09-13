using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Chantier;
using Serilog;

namespace ProjetBase.Businnes.Repositories.Depense
{
    public class DepenseRepository : EntityFrameworkRepository<Entities.Depense, int>, IDepenseRepository
    {

        private readonly IChantierRepository chantierRepository;
        public ProjetBaseContext SbpContext;
        public DepenseRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            SbpContext = dbContext;
            chantierRepository = new ChantierRepository(dbContext);
        }

        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();
                historiqueChamps.ForEach(h =>

                {
                    var dd = h.Attribute;
                    if (h.Attribute != "Articles" && h.Attribute != "Chantier" && h.Attribute != "Prestations"  && h.Attribute != "Tva" && h.Attribute != "Historique" && h.Attribute != "DateCreation" && h.Attribute != "Memos")
                    {
                        if (h.Attribute == "IdChantier")
                        {
                            h.Attribute = "Chantier";
                            int id = Convert.ToInt32(h.After);
                            h.After = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;

                            id = Convert.ToInt32(h.Before);
                            h.Before = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                        }
                        if (h.Attribute == "IdFournisseur")
                        {
                            h.Attribute = "Fournisseur";
                            int id = Convert.ToInt32(h.After);
                            h.After = DbContext.Fournisseurs.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;

                            id = Convert.ToInt32(h.Before);
                            h.Before = DbContext.Fournisseurs.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                        }

                        if (h.Attribute == "Status")
                        {
                            h.After = getStatusDepense(Int32.Parse(h.After));
                            h.Before = getStatusDepense(Int32.Parse(h.Before));

                        }

                        if (h.Attribute == "categorie")
                        {
                            h.After = (h.After == "1" ? "Achat" : "Sous-Traitant");
                            h.Before = (h.Before == "1" ? "Achat" : "Sous-Traitant");
                        }

                        if (h.Attribute == "TotalHt")
                        {
                            double a = Convert.ToDouble(h.After);
                            double b = Convert.ToDouble(h.Before);

                            h.After = String.Format("{0:0.00}", a);
                            h.Before = String.Format("{0:0.00}", b);
                        }
                        if (h.Attribute == "Total")
                        {
                            double a = Convert.ToDouble(h.After);
                            double b = Convert.ToDouble(h.Before);

                            h.After = String.Format("{0:0.00}", a);
                            h.Before = String.Format("{0:0.00}", b);
                        }


                        NewhistoriqueChamps.Add(h);
                    }
                });

                return NewhistoriqueChamps;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Get statut à partir de son numéro
        public string getStatusDepense(int v)
        {
            try
            {
                var statut = "";
                switch (v)
                {
                    case (int)StatutFacture.Brouillon:
                        statut = "Brouillon";
                        break;
                    case (int)StatutFacture.Encours:
                        statut = "En cours";
                        break;
                    case (int)StatutFacture.Cloture:
                        statut = "Clôture";
                        break;
                    case (int)StatutFacture.Enretard:
                        statut = "En retard";
                        break;
                    case (int)StatutFacture.Annule:
                        statut = "Annulée";
                        break;
                    default:
                        statut = "";
                        break;
                }
                return statut;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public Entities.Depense GetDepense(int id)
        {
            var depense = DbContext.Depense.Where(x => x.Id == id)
                            .Include(x=>x.Fournisseur)
                            .Include(x => x.Chantier).ThenInclude(x => x.Client)
                            .Include(x => x.DepenseBonCommandeFournisseurs).ThenInclude(x=> x.BonCommandeFournisseur)
                             .Include(x => x.Paiements).ThenInclude(x => x.ModeReglement)
                            .FirstOrDefault();

            return depense;

        }

  
        public async Task<bool> saveMemos(int id, string memos)
        {
            try
            {
                var depense = GetById(id);
                depense.Memos = memos;
                Update(depense);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
        
        DepenseBonCommandeFournisseur IDepenseRepository.DepenseBonCommandeFournisseur(int idBonCommandeFournisseur, int iddepense , Entities.BonCommandeFournisseur bonCommandeFournisseur)
        {
            try
            {
                var depense =  DbContext.Depense.Where(x => x.Id == iddepense).FirstOrDefault();
                List<DepenseBonCommandeFournisseur> list = new List<DepenseBonCommandeFournisseur>();

                DepenseBonCommandeFournisseur depenseBonCommandeFournisseur = new DepenseBonCommandeFournisseur
                {
                    IdDepense = iddepense,
                    IdBonCommandeFournisseur = idBonCommandeFournisseur,
                  
                };
                list.Add(depenseBonCommandeFournisseur);
                bonCommandeFournisseur.DepenseBonCommandeFournisseurs = list;
                bonCommandeFournisseur.Status = (int)StatutBonCommandeFournisseur.Facturee;
                DbContext.BonCommandeFournisseur.Update(bonCommandeFournisseur);
                return depenseBonCommandeFournisseur;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }
    }
}
