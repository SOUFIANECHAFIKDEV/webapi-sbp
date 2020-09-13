using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using Serilog;

namespace ProjetBase.Businnes.Repositories.Parametrage
{
    public class ParametrageRepository : EntityFrameworkRepository<Entities.Parametrages, int>, IParametrageRepository
    {

        public ParametrageRepository(ProjetBaseContext dbContext) : base(dbContext)
        {

        }

        public async Task<string> GenerateParameter(ProjetBase.Businnes.Models.Parametrage numerotationInfos)
        {
            try
            {
                var generatedParameter = numerotationInfos.Racine;

                var dateToday = DateTime.Today;

                switch (numerotationInfos.FormatDate)
                {
                    case (int)FormatDateParametrage.Annee:
                        generatedParameter += dateToday.Year.ToString();
                        break;

                    case (int)FormatDateParametrage.AnneeMois:
                        generatedParameter += dateToday.Year.ToString() + dateToday.Month.ToString();
                        break;

                    default:
                        break;
                }

                generatedParameter += generatePartieCompteur(numerotationInfos.Compteur, numerotationInfos.LongeurCompteur);

                return generatedParameter;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }


        public string generatePartieCompteur(int compteur, int? longeurCompteur)
        {
            if (longeurCompteur.HasValue)
            {
                return compteur.ToString().PadLeft(longeurCompteur.Value, '0');
            }
            else
            {
                return compteur.ToString();
            }
        }

        public Parametrages GetParametrageDocument()
        {
            return DbContext.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.parametrageDocument)).FirstOrDefault();
        }


        public Parametrages GetParametrageMessagerie()
        {
            return DbContext.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.configMessagerie)).FirstOrDefault();

        }

        async public Task<bool> Increment(TypeNumerotation type)
        {
            try
            {
                var parametrage = DbContext.Parametrages.AsNoTracking().SingleOrDefault(x => x.Type == (int)TypeParametrage.numerotaion);

                List<Models.Parametrage> contenu = JsonConvert.DeserializeObject<List<Models.Parametrage>>(parametrage.Contenu);

                if (parametrage == null)
                {
                    return false;
                }
                foreach (var numerotation in contenu)
                {
                    if (numerotation.Type == (int)type)
                    {
                        // Incrémenter le compteur
                        numerotation.Compteur++;
                    }
                }
                parametrage.Contenu = JsonConvert.SerializeObject(contenu);
                DbContext.Parametrages.Update(parametrage);
                await DbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
            Log.Error(ex, ex.Message);
            
            throw ex;
            }
        }

        async public Task<Parametrages> IncrementReference(Parametrages parametrage, TypeNumerotation type)
        {
            try
            {
                List<Models.Parametrage> contenu = JsonConvert.DeserializeObject<List<Models.Parametrage>>(parametrage.Contenu);

                if (parametrage == null)
                {
                    return null;
                }

                foreach (var numerotation in contenu)
                {
                    if (numerotation.Type == (int)type)
                    {
                        // Incrémenter le compteur
                        numerotation.Compteur++;
                    }
                }

                parametrage.Contenu = JsonConvert.SerializeObject(contenu);

                DbContext.Parametrages.Update(parametrage);
                await DbContext.SaveChangesAsync();

                return parametrage;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                throw ex;
            }
        }

    }
}
