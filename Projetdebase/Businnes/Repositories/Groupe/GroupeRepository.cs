using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Groupe
{
    public class GroupeRepository : EntityFrameworkRepository<Entities.Groupe, int>, IGroupeRepository
    {
        public GroupeRepository(ProjetBaseContext dbContext) : base(dbContext)
        {

        }

        public bool CheckUniqueNom(string nom)
        {
            var NbrReference = DbContext.Groupes.Where(x => x.Nom == nom).Count();
            return NbrReference > 0;
        }

        public Entities.Groupe GetGroupe(int id)
        {
            try
            {
                          var groupe = DbContext.Groupes.Where(x=> x.Id == id)
                                                       .Include(x => x.Clients)
                                                       .FirstOrDefault();
                return groupe;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /*
          try
            {
                var devis = DbContext.Devis.Where(x => x.Id == id)
                                        .Include(x => x.Chantier).ThenInclude(x => x.Client)
                                        .Include(x=>x.BonCommandeFournisseur)
                                        .Include(x => x.Facture).ThenInclude(x => x.FacturePaiements).ThenInclude(x => x.Paiement).ThenInclude(x => x.ModeReglement)
                                        .FirstOrDefault();
                return devis;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }*/
    }

}
