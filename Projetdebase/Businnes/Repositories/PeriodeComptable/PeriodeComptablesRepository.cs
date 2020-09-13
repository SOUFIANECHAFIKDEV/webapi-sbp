using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.PeriodeComptable
{
    public class PeriodeComptablesRepository : EntityFrameworkRepository<Entities.PeriodeComptable, int>, IPeriodeComptablesRepository
    {
        public PeriodeComptablesRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }

        public async Task CloturePeriode(int id)
        {
            try
            {
                //Current User
                var currentUser =  EntityExtensions.GetCurrentUser(DbContext);

                // Séléctionné période comptable et set la date cloture est aujourd'hui
                var periodeComptable = DbContext.PeriodeComptables.Where(x => x.Id == id).FirstOrDefault();
                periodeComptable.DateCloture = DateTime.Now;
                periodeComptable.IdUser = currentUser.Id;
                DbContext.PeriodeComptables.Update(periodeComptable);

                //Range des dates
                var dateMin = periodeComptable.DateDebut.Date;
                var dateMax = periodeComptable.DateDebut.Date.AddMonths(periodeComptable.Periode).AddDays(-1);

                //Factures
                var facturesInPeriode = await DbContext.Factures.Where(x => x.DateCreation.Date <= dateMax && x.DateCreation.Date >= dateMin).ToListAsync();
                facturesInPeriode.ForEach(x => x.Comptabilise = (int)StatutComptabilise.Oui);
                DbContext.Factures.UpdateRange(facturesInPeriode);

                //Avoirs
                var avoirInPeriode = await DbContext.Avoirs.Where(x => x.DateCreation.Date <= dateMax && x.DateCreation >= dateMin).ToListAsync();
                avoirInPeriode.ForEach(x => x.Comptabilise = (int)StatutComptabilise.Oui);
                DbContext.Avoirs.UpdateRange(avoirInPeriode);

                //Paiements
                var paiementInPeriode = await DbContext.Paiements.Where(x => x.DatePaiement.Date <= dateMax && x.DatePaiement >= dateMin).ToListAsync();
                paiementInPeriode.ForEach(x => x.Comptabilise = (int)StatutComptabilise.Oui);
                DbContext.Paiements.UpdateRange(paiementInPeriode);

                //Depense
                var depenseInPeriode = await DbContext.Depense.Where(x => x.DateCreation.Date <= dateMax && x.DateCreation >= dateMin).ToListAsync();
                depenseInPeriode.ForEach(x => x.Comptabilise = (int)StatutComptabilise.Oui);
                DbContext.Depense.UpdateRange(depenseInPeriode);

                await DbContext.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                throw ex;
            }
        }
    }
}
