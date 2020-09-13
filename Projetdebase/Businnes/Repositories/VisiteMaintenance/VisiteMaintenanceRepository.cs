using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.VisiteMaintenance
{
    public class VisiteMaintenanceRepository : EntityFrameworkRepository<Entities.VisiteMaintenance, int>, IVisiteMaintenanceRepository
    {
        public VisiteMaintenanceRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }

        public async Task<bool> AddVisiteMaintenance(Entities.ContratEntretien contratEntretien)
        {
            try
            {
                List<Entities.VisiteMaintenance> visiteMaintenances = new List<Entities.VisiteMaintenance>();
                int years = (contratEntretien.DateFin.Year - contratEntretien.DateDebut.Year) + 1;
                var FirstYears = contratEntretien.DateDebut.Year;
                for (int year = 0; year < years; year++)
                {
                    for (int month = 0; month < 12; month++)
                    {
                        var dateVisite = new DateTime(FirstYears + year, month + 1, 1);
                        var datedebut = new DateTime(contratEntretien.DateDebut.Year, contratEntretien.DateDebut.Month, 1);
                        var dateFin = new DateTime(contratEntretien.DateFin.Year, contratEntretien.DateFin.Month, 1);
                        var minDate = DateTime.Compare(dateVisite, datedebut);
                        var maxDate = DateTime.Compare(dateVisite, dateFin);

                        if (minDate != -1 && maxDate != 1)
                        {
                            var Allperiodicites = contratEntretien.EquipementContrat.SelectMany(x => x.Libelle
                                .SelectMany(a => a.OperationsEquipement.SelectMany(d => d.Periodicite))).ToList();

                            //var janvier = Allperiodicites.Where(x => x.Mois == (month + 1)).ToList();

                            var contarts = Allperiodicites.Where(x => x.Mois == (month + 1)).ToList()
                                .Select(x => x.OperationsEquipement.LibelleEquipement.EquipementContrat).Distinct().ToList();

                            List<Models.GammeMaintenanceVisiteModel> GammeMaintenanceListe = new List<Models.GammeMaintenanceVisiteModel>();

                            foreach (var contart in contarts)
                            {
                                List<Models.LibelleModel> libellesListe = new List<Models.LibelleModel>();
                                List<Models.OperationsModel> operationsListe = new List<Models.OperationsModel>();
                                var libelles = contart.Libelle.Distinct().ToList();
                                foreach (var libelle in libelles)
                                {
                                    var operations = libelle.OperationsEquipement.Where(x => x.Periodicite.Where(ss => ss.Mois == (month + 1)).Count() > 0).ToList();
                                    if (operations.Count > 0)
                                    {
                                        //var ops = libelle.OperationsEquipement.Distinct().ToList();
                                        //foreach(var operation in operations)
                                        //{
                                        //    operationsListe.Add(new Models.OperationsModel
                                        //    {
                                        //        id = operation.Id,
                                        //        nom = operation.Nom,
                                        //    });

                                        //};

                                        //libellesListe.Add(new Models.LibelleModel
                                        //{
                                        //    id = libelle.Id,
                                        //    nom = libelle.Nom,
                                        //    operations = operationsListe 
                                        //});
                                       
                                            libellesListe.Add(new Models.LibelleModel
                                            {
                                                nom = libelle.Nom,
                                                operations = operations.Select(x => new Models.OperationsModel {
                                                    id = x.Id,
                                                    nom = x.Nom,
                                                    
                                                }).ToList()
                                            });
                                        
                                    }
                                }
                                GammeMaintenanceListe.Add(new Models.GammeMaintenanceVisiteModel
                                {
                                    id = contart.Id,
                                    nom = contart.Nom,
                                    libelle = libellesListe
                                });
                            }

                            if (GammeMaintenanceListe.Count() > 0)
                            {
                                visiteMaintenances.Add(new Entities.VisiteMaintenance
                                {
                                    Annee = FirstYears + year,
                                    Mois = month + 1,
                                    Statut = (int)Enum.StatutVisiteMaintenance.APlanifier,
                                    GammeMaintenance = JsonConvert.SerializeObject(GammeMaintenanceListe),
                                    IdContratEntretien = contratEntretien.Id,
                                });
                            }
                        }
                    }
                }

                DbContext.VisiteMaintenance.AddRange(visiteMaintenances);
                return await DbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public PagedList<Entities.VisiteMaintenance> Filter(PagingParams pagingParams, Expression<Func<Entities.VisiteMaintenance, bool>> filter = null, SortingParams sortingParams = null)
        //{
        //    try
        //    {
        //        IQueryable<Entities.VisiteMaintenance> query = DbContext.Set<Entities.VisiteMaintenance>();
        //        query = query.Where(filter).Include(f => f.ContratEntretien).ThenInclude(x=>x.Client)
        //                                  .Include(f=>f.FicheInterventionMaintenance);



        //        return new PagedList<Entities.VisiteMaintenance>(query, pagingParams.PageNumber, pagingParams.PageSize);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
        //public PagedList<Entities.VisiteMaintenance> Filter(PagingParams pagingParams, Expression<Func<Entities.VisiteMaintenance, bool>> filter = null, SortingParams sortingParams = null)
        //{
        //    IQueryable<Entities.VisiteMaintenance> query = DbContext.Set<Entities.VisiteMaintenance>();
        //    query = query.Where(filter).Include(x => x.ContratEntretien).ThenInclude(x => x.Client)
        //            .Include(u => u.FicheInterventionMaintenance)
        //            .Select(f => new Entities.VisiteMaintenance()
        //            {
        //                id = f.Annee,
        //                IdContratEntretien = f.IdContratEntretien,
        //                Annee = f.Annee,
        //                GammeMaintenance = f.GammeMaintenance,
        //                Mois = f.Mois,
        //                Statut = f.Statut,
        //                ContratEntretien = f.ContratEntretien,
        //                FicheInterventionMaintenance = f.FicheInterventionMaintenance,


        //            });

        //    if (sortingParams != null)
        //    {
        //        query = new SortedList<Entities.VisiteMaintenance>(query, sortingParams).GetSortedList();
        //    }

        //    return new PagedList<Entities.VisiteMaintenance>(query, pagingParams.PageNumber, pagingParams.PageSize);
        //}

        public PagedList<Entities.VisiteMaintenance> Filter(PagingParams pagingParams, Expression<Func<Entities.VisiteMaintenance, bool>> filter = null, SortingParams sortingParams = null)
        {
            IQueryable<Entities.VisiteMaintenance> query = DbContext.Set<Entities.VisiteMaintenance>();
            query = query.Where(filter).Include(f => f.ContratEntretien).ThenInclude(u => u.Client).Include(x => x.FicheInterventionMaintenance)/*.Include(x => x.Pays)*/;
            if (sortingParams != null)
            {
                query = new SortedList<Entities.VisiteMaintenance>(query, sortingParams).GetSortedList();
            }

            return new PagedList<Entities.VisiteMaintenance>(query, pagingParams.PageNumber, pagingParams.PageSize);
        }


        public Entities.VisiteMaintenance GetVisiteMaintenance(int id)
        {
            try
            {
                var VisiteMaintenance = DbContext.VisiteMaintenance.Where(x => x.id == id)
                                        .Include(x => x.ContratEntretien).ThenInclude(x=>x.Client)

                                       // .Include(x => x.ContratEntretien).ThenInclude(x => x.EquipementContrat).ThenInclude(x => x.Libelle).ThenInclude(x => x.OperationsEquipement).ThenInclude(x => x.Periodicite)
                                        .FirstOrDefault();
                return VisiteMaintenance;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }


        int Years(DateTime start, DateTime end)
        {
            return (end.Year - start.Year - 1) +
                (((end.Month > start.Month) ||
                ((end.Month == start.Month) && (end.Day >= start.Day))) ? 1 : 0);
        }
    }
}
