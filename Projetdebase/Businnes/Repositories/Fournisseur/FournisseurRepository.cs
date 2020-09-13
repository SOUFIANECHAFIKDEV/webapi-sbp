using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Models;
using Serilog;

namespace ProjetBase.Businnes.Repositories.Fournisseur
{
    public class FournisseurRepository : EntityFrameworkRepository<Entities.Fournisseur, int>, IFournisseurRepository
    {
        public FournisseurRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }

        public bool CheckUniqueReference(string reference)
        {
            var NbrReference = DbContext.Fournisseurs.Where(x => x.Reference == reference).Count();
            return NbrReference > 0;
        }

        /*
         * Save Memos de fournisseur
         */
        public async Task<bool> saveMemos(int id, string memos)
        {
            try
            {
                var fourniseur = GetById(id);
                fourniseur.Memos = memos;
                Update(fourniseur);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        /*
         *  Changer les ids à des noms
         */
        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                historiqueChamps.ForEach(h =>
                {
                    if (h.Attribute == "IdPays")
                    {
                        h.Attribute = "Pays";
                        if (h.Before != "" && h.Before != null)
                        {
                            h.Before = DbContext.Pays.Where(pays => pays.Id == Int32.Parse(h.Before)).FirstOrDefault().NomFrFr;
                        }

                        if (h.After != "" && h.After != null)
                        {
                            h.After = DbContext.Pays.Where(pays => pays.Id == Int32.Parse(h.After)).FirstOrDefault().NomFrFr;
                        }
                    }
                    if(h.Attribute == "IdAgent")
                    {
                        h.Attribute = "Agent";
                        if (h.Before != "" && h.Before != null)
                        {
                            var user = DbContext.User.Where(x => x.Id == Int32.Parse(h.Before)).FirstOrDefault();
                            h.Before = string.Join(" ", user.Nom , user.Prenom);
                        }

                        if (h.After != "" && h.After != null)
                        {
                            var user = DbContext.User.Where(x => x.Id == Int32.Parse(h.After)).FirstOrDefault();
                            h.After = string.Join(" ", user.Nom, user.Prenom);
                        }
                    }
                });

                return historiqueChamps;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public bool supprimerFournisseur(int id)
        {
            try
            {
                //vérifier que fournisseur pas de prestation
                if (DbContext.ProduitFournisseur.Where(PF => PF.idFournisseur == id).Count() != 0)
                {
                    return false;
                }
                if (DbContext.Depense.Where(PF => PF.IdFournisseur == id).Count() != 0)
                {
                    return false;
                }
                if (DbContext.BonCommandeFournisseur.Where(PF => PF.IdFournisseur == id).Count() != 0)
                {
                    return false;
                }
                //supprimer le fournisseur
                var fournisseur = DbContext.Fournisseurs.SingleOrDefault(F => F.Id == id);
                DbContext.Fournisseurs.Remove(fournisseur);
                DbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
