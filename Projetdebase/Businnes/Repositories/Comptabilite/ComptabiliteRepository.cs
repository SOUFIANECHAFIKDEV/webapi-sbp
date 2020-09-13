using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using ProjetBase.Businnes.Outils.EXCEL;

using Serilog;

namespace ProjetBase.Businnes.Repositories.Comptabilite
{
    public class ComptabiliteRepository : IComptabiliteRepository
    {

        private readonly ProjetBaseContext _context;
        private readonly IGenerateEXCEL _generateEXCEL;
       

        public ComptabiliteRepository(ProjetBaseContext context)
        {
            _context = context;
            _generateEXCEL = new GenerateEXCEL(_context);
         
        }
        public async Task<PagedComptabiliteList<JournalBanqueModel>> ComptabiliteComptes(ComptabiliteComptesFilter filterModel)
        {
            try
            {
                var query = await FilterComptabiliteComptes(filterModel);
                if (filterModel.SortingParams != null)
                {
                    query = new SortedList<Entities.Paiement>(query, filterModel.SortingParams).GetSortedList();
                }
                var res = new PagedList<Entities.Paiement>(query, filterModel.PagingParams.PageNumber, filterModel.PagingParams.PageSize);
                var listFormaterComptabiliteCompte = FormaterComptebiliteComptes(filterModel, res.List);
                return new PagedComptabiliteList<JournalBanqueModel>()
                {
                    List = listFormaterComptabiliteCompte,
                    PageNumber = res.PageNumber,
                    PageSize = res.PageSize,
                    TotalItems = res.TotalItems
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Filter les données de journal banque
        public async Task<IQueryable<Entities.Paiement>> FilterComptabiliteComptes(ComptabiliteComptesFilter filterModel)
        {
            try
            {
                if (filterModel.Periode == (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT)
                {
                    var periodeComptableActual = await _context.PeriodeComptables.OrderByDescending(x => x.DateDebut).Where(p => !p.DateCloture.HasValue).FirstOrDefaultAsync();

                    if (periodeComptableActual != null)
                    {
                        filterModel.DateMinimal = periodeComptableActual.DateDebut;
                        filterModel.DateMaximale = periodeComptableActual.DateDebut.AddMonths(periodeComptableActual.Periode).AddDays(-1);
                    }
                }

                IQueryable<Entities.Paiement> query = _context.Paiements
                    .Include(f => f.FacturePaiements).ThenInclude(f => f.Facture)
                    .Include(f => f.Depense).ThenInclude(f => f.Chantier).ThenInclude(f => f.Client)
                    .Include(f => f.Depense).ThenInclude(f => f.Fournisseur)
                    .Include(f => f.ModeReglement)
                    .Include(f => f.ParametrageCompte)
                     .Where(x =>
                    ((filterModel.Periode != (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT && filterModel.Periode != (int)PeriodeEnum.INTERVALLE) || filterModel.DateMinimal == null ? true : filterModel.DateMinimal <= x.DatePaiement.Date) &&
                    ((filterModel.Periode != (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT && filterModel.Periode != (int)PeriodeEnum.INTERVALLE) || filterModel.DateMaximale == null ? true : filterModel.DateMaximale >= x.DatePaiement.Date) &&
                    (filterModel.Periode == (int)PeriodeEnum.ANNEE_ACTUAL ? x.DatePaiement.Date.Year == DateTime.Now.Year : true) &&
                    (filterModel.Periode == (int)PeriodeEnum.ANNEE_DERNIERE ? x.DatePaiement.Date.Year == (DateTime.Now.Year - 1) : true) &&
                    (filterModel.Periode == (int)PeriodeEnum.MOIS_ACTUAL ? x.DatePaiement.Date.Month == DateTime.Now.Month && x.DatePaiement.Date.Year == DateTime.Now.Year : true) &&
                    (filterModel.Periode == (int)PeriodeEnum.SIX_DERNIERE_MOIS ? (EntityExtensions.GetDateForMonth(-6) <= x.DatePaiement.Date && EntityExtensions.GetDateForMonth(DateTime.Now.Month) >= x.DatePaiement.Date) : true) &&
                    (filterModel.Periode == (int)PeriodeEnum.TROIS_DERNIERE_MOIS ? (EntityExtensions.GetDateForMonth(-3) <= x.DatePaiement.Date && EntityExtensions.GetDateForMonth(DateTime.Now.Month) >= x.DatePaiement.Date) : true) &&
                    (filterModel.IsCaisse ? x.ParametrageCompte.Type == (int)TypeCompte.caisse : x.ParametrageCompte.Type != (int)TypeCompte.caisse) &&

                    x.ModeReglement.Id != (int)StaticModeReglement.Avoir
                      // (
                      //&& (x.FacturePaiements.Any(f => (f.Facture.Reference ?? "").Contains(filterModel.SearchQuery))) 
                      //||
                      // (x.Depense != null ? (x.Depense.Reference.Contains(filterModel.SearchQuery)) : true)
                      //)
                      // && (x.FacturePaiements.Where(x=>x.Facture.Reference )
                      );

                List<Entities.Paiement> myQuery = query.ToList();
                myQuery = myQuery.Where(x =>
                    x.FacturePaiements.Any(f => (f.Facture.Reference ?? "").Contains(filterModel.SearchQuery))
                   || (x.IdDepense == null ? true : ((x.Depense.Reference).Contains(filterModel.SearchQuery)))
                ).ToList();
                //myQuery = myQuery.Where(x =>

                //x.FacturePaiements.Any(f => (f.Facture.Reference ?? "").Contains(filterModel.SearchQuery))
                ////&&
                ////(x.Depense != null ? ((x.Depense.Reference ?? "" ).Contains(filterModel.SearchQuery)) : true)
                //);

                query = myQuery.AsQueryable();


                return query;
            }
            catch (Exception ex)

            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Formater les données journal banque
        public List<JournalBanqueModel> FormaterComptebiliteComptes(ComptabiliteComptesFilter filterModel, List<Entities.Paiement> paiements)
        {
            try
            {
                var journalItems = new List<JournalBanqueModel>();
                var ListPlanComptable = _context.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.planComptable)).FirstOrDefault();
                List<PlanComptabeModel> PlanComptabe = JsonConvert.DeserializeObject<List<PlanComptabeModel>>(ListPlanComptable.Contenu);
                int idv = (int)PlanComptableEnum.virements_internes;
                var codeComptableVirementsInternes = PlanComptabe.Where(x => x.id == idv).SingleOrDefault();

                foreach (var item in paiements)
                {
                    foreach (var facturePaiement in item.FacturePaiements)
                    {
                        journalItems.Add(new JournalBanqueModel()
                        {
                            CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),
                            DatePaiement = item.DatePaiement,
                            NumeroCompte = item.ParametrageCompte.code_comptable,
                            NumeroPiece = facturePaiement.Facture.Reference,
                            Tiers = item.ParametrageCompte.Nom,
                            Debit = facturePaiement.Montant.RoundingDouble(),
                            Credit = 0,
                            TypePaiement = item.ModeReglement.Nom
                        });
                        journalItems.Add(new JournalBanqueModel()
                        {
                            CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),
                            DatePaiement = item.DatePaiement,
                            NumeroCompte = _context.Clients.Where(c => c.Id == facturePaiement.Facture.IdClient).Select(x => x.CodeComptable).FirstOrDefault(),
                            NumeroPiece = facturePaiement.Facture.Reference,
                            Tiers = _context.Clients.Where(c => c.Id == facturePaiement.Facture.IdClient).Select(x => x.Nom).FirstOrDefault(),
                            Debit = 0,
                            Credit = facturePaiement.Montant.RoundingDouble(),
                            TypePaiement = item.ModeReglement.Nom

                        });
                    }
                    if (item.IdDepense != null)
                    {
                        journalItems.Add(new JournalBanqueModel()
                        {
                            CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),
                            DatePaiement = item.DatePaiement,
                            NumeroCompte = item.ParametrageCompte.code_comptable,
                            NumeroPiece = item.Depense.Reference,
                            Tiers = item.ParametrageCompte.Nom,
                            Debit = item.Montant.RoundingDouble(),
                            Credit = 0,
                            TypePaiement = item.ModeReglement.Nom
                        });
                        journalItems.Add(new JournalBanqueModel()
                        {
                            CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),
                            DatePaiement = item.DatePaiement,
                            NumeroCompte = item.Depense.Fournisseur.CodeComptable,
                            NumeroPiece = item.Depense.Reference,
                            Tiers = item.Depense.Fournisseur.Nom,
                            Debit = 0,
                            Credit = item.Montant.RoundingDouble(),
                            TypePaiement = item.ModeReglement.Nom

                        });

                    }
                  if (item.IdDepense == null && item.FacturePaiements.Count() == 0)
                    {
                      if (item.type ==(int)TypePaiement.VIREMENT_A)
                        {
                            journalItems.Add(new JournalBanqueModel()
                            {
                                CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),

                                DatePaiement = item.DatePaiement,
                                NumeroCompte = item.ParametrageCompte.code_comptable,
                                NumeroPiece = item.ParametrageCompte.Nom,
                                Tiers = item.ParametrageCompte.Nom,
                                Debit = item.Montant.RoundingDouble(),
                                Credit = 0,
                                TypePaiement = item.ModeReglement.Nom
                            });
                          
                            journalItems.Add(new JournalBanqueModel()
                            {
                             
                                CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),
                                DatePaiement = item.DatePaiement,
                                NumeroCompte = codeComptableVirementsInternes.codeComptable.ToString(),
                                NumeroPiece = item.ParametrageCompte.Nom,
                                Tiers = item.ParametrageCompte.Nom,
                                Debit = 0,
                                Credit = item.Montant.RoundingDouble(),
                                TypePaiement = item.ModeReglement.Nom

                            });
                        }

                        if (item.type == (int)TypePaiement.VIREMENT_DEPUIS)
                        {
                            journalItems.Add(new JournalBanqueModel()
                            {
                                CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),

                                DatePaiement = item.DatePaiement,
                                NumeroCompte = codeComptableVirementsInternes.codeComptable.ToString(),
                                NumeroPiece = item.ParametrageCompte.Nom,
                                Tiers = item.ParametrageCompte.Nom,
                                Debit =  (item.Montant.RoundingDouble()) * (-1),
                                Credit = 0,
                                TypePaiement = item.ModeReglement.Nom
                            });
                          
                            journalItems.Add(new JournalBanqueModel()
                            {

                                CodeJournal = (filterModel.IsCaisse ? "CAISSE" : "BANQUE"),
                                DatePaiement = item.DatePaiement,
                                NumeroCompte = item.ParametrageCompte.code_comptable ,
                                NumeroPiece = item.ParametrageCompte.Nom,
                                Tiers = item.ParametrageCompte.Nom,
                                Debit = 0,
                                Credit =  (item.Montant.RoundingDouble()) * (-1),
                                TypePaiement = item.ModeReglement.Nom

                            });
                        }



                    }

                }
                return journalItems;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        //Journal de vente avec pagination
        public async Task<PagedComptabiliteList<JournalVenteModel>> JournalVente(JournalVenteFilter filterModel)
        {
            try
            {
                var query = await FilterJournalVente(filterModel);
                if (filterModel.SortingParams != null)
                {
                    query = new SortedList<JournalVenteSelectModel>(query, filterModel.SortingParams).GetSortedList();
                }
                var res = new PagedList<JournalVenteSelectModel>(query, filterModel.PagingParams.PageNumber, filterModel.PagingParams.PageSize);
                var listFormaterJournalVente = await FormaterJournalVente(filterModel, res.List);

                return new PagedComptabiliteList<JournalVenteModel>()
                {
                    List = listFormaterJournalVente,
                    PageNumber = res.PageNumber,
                    PageSize = res.PageSize,
                    TotalItems = res.TotalItems
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }


        public async Task<IQueryable<JournalVenteSelectModel>> FilterJournalVente(JournalVenteFilter filterModel)
        {
            try
            {

                if (filterModel.Periode == (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT)
                {
                    var periodeComptableActual = await _context.PeriodeComptables.OrderByDescending(x => x.DateDebut).Where(p => !p.DateCloture.HasValue).FirstOrDefaultAsync();

                    if (periodeComptableActual != null)
                    {
                        filterModel.DateMinimal = periodeComptableActual.DateDebut;
                        filterModel.DateMaximale = periodeComptableActual.DateDebut.AddMonths(periodeComptableActual.Periode).AddDays(-1);
                    }
                }

                IQueryable<JournalVenteSelectModel> query = _context.Factures.Where(x => x.Status != (int)StatutFacture.Brouillon)
                    .Select(facture => new JournalVenteSelectModel
                    {
                        Id = facture.Id,
                        DateCreation = facture.DateCreation,
                        Reference = facture.Reference,
                        NomClient = _context.Clients.Where(c => c.Id == facture.IdClient).Select(x => x.Nom).FirstOrDefault(),
                        CodeComptableClient = _context.Clients.Where(c => c.Id == facture.IdClient).Select(x => x.CodeComptable).FirstOrDefault(),
                        Articles = facture.Prestations,
                        TotalTTC = facture.Total,
                        Type = (int)TypeItemJournalVente.FACTURE,
                        TotalHT = facture.TotalHt,
                        TVA = facture.Tva,
                        Remise = facture.Remise,
                        TypeRemise = facture.TypeRemise
                    })
                    .Union(
                    _context.Avoirs.Where(x => x.Status != (int)StatutAvoir.Brouillon)
                    .Select(avoir => new JournalVenteSelectModel
                    {
                        Id = avoir.Id,
                        DateCreation = avoir.DateCreation,
                        Reference = avoir.Reference,
                        NomClient = _context.Clients.Where(c => c.Id == avoir.IdClient).Select(x => x.Nom).FirstOrDefault(),
                        CodeComptableClient = _context.Clients.Where(c => c.Id == avoir.IdClient).Select(x => x.CodeComptable).FirstOrDefault(),
                        Articles = avoir.Prestations,
                        TotalTTC = avoir.Total,
                        Type = (int)TypeItemJournalVente.AVOIR,
                        TotalHT = avoir.TotalHt,
                        TVA = avoir.Tva,
                        Remise = avoir.Remise,
                        TypeRemise = avoir.TypeRemise
                    }))
                    .Where(x =>
                        (x.Reference.Contains(filterModel.SearchQuery)) &&
                        ((filterModel.Periode != (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT && filterModel.Periode != (int)PeriodeEnum.INTERVALLE) || filterModel.DateMinimal == null ? true : filterModel.DateMinimal <= x.DateCreation.Date) &&
                        ((filterModel.Periode != (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT && filterModel.Periode != (int)PeriodeEnum.INTERVALLE) || filterModel.DateMaximale == null ? true : filterModel.DateMaximale >= x.DateCreation.Date) &&
                        (filterModel.Periode == (int)PeriodeEnum.ANNEE_ACTUAL ? x.DateCreation.Date.Year == DateTime.Now.Year : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.ANNEE_DERNIERE ? x.DateCreation.Date.Year == (DateTime.Now.Year - 1) : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.MOIS_ACTUAL ? x.DateCreation.Date.Month == DateTime.Now.Month && x.DateCreation.Date.Year == DateTime.Now.Year : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.SIX_DERNIERE_MOIS ? (EntityExtensions.GetDateForMonth(-6) <= x.DateCreation.Date && EntityExtensions.GetDateForMonth(DateTime.Now.Month) >= x.DateCreation.Date) : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.TROIS_DERNIERE_MOIS ? (EntityExtensions.GetDateForMonth(-3) <= x.DateCreation.Date && EntityExtensions.GetDateForMonth(DateTime.Now.Month) >= x.DateCreation.Date) : true)

                    )
                    .OrderBy(x => x.DateCreation);

                return query;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

    
        // Formater les données journal de vente
        public async Task<List<JournalVenteModel>> FormaterJournalVente(JournalVenteFilter filterModel, List<JournalVenteSelectModel> journalVentes)

        {
            try
            {
           
                var journalItems = new List<JournalVenteModel>();
                if(journalVentes.Count != 0)
                {
                    var categories = _context.Categorie.ToList();
                    foreach (var item in journalVentes)
                    {

                        List<PrestationsModule> prestations = JsonConvert.DeserializeObject<List<PrestationsModule>>(item.Articles);
                        List<Data> ListPrestationsInLot = new List<Data>();

                        var prestationsInLot = prestations.Where(P => P.data.lotProduits != null).ToList();
                        foreach (var p in prestationsInLot)
                        {
                            List<Data> fournisseurPrestationsInLot = new List<Data>();
                            Data DataPrestations = p.data;
                            List<DataLotProduit> dataLotProduit = DataPrestations.lotProduits;
                            fournisseurPrestationsInLot = dataLotProduit.Select(e => e.idProduitNavigation).ToList();

                            ListPrestationsInLot.AddRange(fournisseurPrestationsInLot);

                        }
                        var ListPrestations = ListPrestationsInLot.Concat(prestations.Where(P => P.data.lotProduits == null).Select(e => e.data).ToList()).ToList();
                        // var calculGenerale = calculGenerale(ListPrestations, item.Remise, item.TypeRemise);
                        var totalHtvv = totalHt(ListPrestations);
                        var groupArticles = GroupeArticleByCategories(ListPrestations, categories, item.Remise, item.TypeRemise, totalHtvv);

                        journalItems.Add(new JournalVenteModel()
                        {
                            CodeJournal = "VENTE",
                            DateCreation = item.DateCreation,
                            NumeroCompte = item.CodeComptableClient,
                            NumeroPiece = item.Reference,
                            NomClient = item.NomClient,
                            Debit = item.TotalTTC.RoundingDouble(),
                            Credit = 0,

                        });

                        List<TvaModel> ListTva = JsonConvert.DeserializeObject<List<TvaModel>>(item.TVA);

                        foreach (var tvaA in ListTva)
                        {

                            var ListPlanComptable = _context.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.tvaModel)).FirstOrDefault();
                            List<PlanComptabeTvaModel> PlanComptabe = JsonConvert.DeserializeObject<List<PlanComptabeTvaModel>>(ListPlanComptable.Contenu);
                            PlanComptabeTvaModel planComptabeTva = PlanComptabe.Where(plan => plan.tva == System.Convert.ToString(tvaA.tva)).FirstOrDefault();
                            double codeComptable = 0;
                            if (planComptabeTva == null)
                            {
                                codeComptable = PlanComptabe.Where(plan => plan.tva == "defaut").Select(x => x.codeComptable).FirstOrDefault();
                            }
                            else
                            {
                                codeComptable = planComptabeTva.codeComptable;
                            }

                            if (tvaA.totalTVA != 0)
                            {


                                journalItems.Add(new JournalVenteModel()
                                {
                                    CodeJournal = "VENTE",
                                    DateCreation = item.DateCreation,
                                    NumeroCompte = codeComptable.ToString()/*tvas.racine + (tvas.tvalist.Where(t => t.valeurtva == tva.tva).FirstOrDefault()?.codecomptable ?? "")*/,
                                    NumeroPiece = item.Reference,
                                    NomClient = item.NomClient,
                                    Debit = 0,
                                    Credit = tvaA.totalTVA.RoundingDouble(),
                                });
                            }
                        }

                        foreach (var groupArticle in groupArticles)
                        {
                            //if(groupArticle.CodeComptable != null && groupArticle.Total !=0)
                            //{
                            journalItems.Add(new JournalVenteModel()
                            {
                                CodeJournal = "VENTE",
                                DateCreation = item.DateCreation,
                                NumeroCompte = groupArticle.CodeComptable,
                                NumeroPiece = item.Reference,
                                NomClient = item.NomClient,
                                Debit = 0,
                                Credit = groupArticle.Total.RoundingDouble()
                            });
                            //}

                        }
                    }
                }
            
                return journalItems;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }
        // Groupe articles par categorie
        public List<GroupeArticle> GroupeArticleByCategories(List<Data> articles, List<Entities.Categorie> categories, double remise, string typeRemise, double totalSansRemise)
        {
            try
            {
                var groupeArticles = articles.GroupBy(x => x.categorie).Select(x => new GroupeArticle
                {

                    Total = totalHTArticleComptabilite(x.Sum(article => article.prixHt * article.qte), totalSansRemise, remise, typeRemise),
                    CodeComptable = categories.Where(cat => cat.Nom == x.FirstOrDefault().categorie).Select(p => p.Code_comptable).FirstOrDefault(),

                    //(string.IsNullOrEmpty(
                    //    categories.Where(cat => cat.Nom == x.FirstOrDefault().categorie).FirstOrDefault()?.Code_comptable) ? categories.Where(cat => cat.Nom == "Autre").FirstOrDefault().Code_comptable : categories.Where(cat => cat.Nom == x.FirstOrDefault().categorie).FirstOrDefault()?.Code_comptable)
                }).ToList();

                var cc = groupeArticles;
                return groupeArticles;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public double totalHTArticleComptabilite(double prixTotalHtArticle, double totalHtSansRemise, double remiseGlobal, string typeRemiseGlobal)
        {
            if (remiseGlobal != 0)
            {
                if (typeRemiseGlobal != "%")
                {
                    return prixTotalHtArticle - ((prixTotalHtArticle / totalHtSansRemise) * remiseGlobal);
                }
                else
                {
                    return prixTotalHtArticle - ((remiseGlobal * prixTotalHtArticle) / 100);
                }
            }
            else
            {
                return prixTotalHtArticle;
            }
        }

        public double totalHt(List<Data> articles)
        {
            return articles.Sum(x => (x.qte) * (x.prixHt));

        }
        public double totalHtDepense(List<Data> articles, int? idFournisseur)
        {

            return articles.Sum(x => (x.qte) * x.prixParFournisseur.Where(y => y.idFournisseur == idFournisseur).Select(y => y.prix).SingleOrDefault());

        }

        public async Task<PagedComptabiliteList<JournalVenteModel>> JournalAchat(JournalVenteFilter filterModel)
        {
            try
            {
                var query = await FilterJournalAchat(filterModel);

                if (filterModel.SortingParams != null)
                {
                    query = new SortedList<JournalVenteSelectModel>(query, filterModel.SortingParams).GetSortedList();
                }

                var res = new PagedList<JournalVenteSelectModel>(query, filterModel.PagingParams.PageNumber, filterModel.PagingParams.PageSize);

                var listFormaterJournalVente = await FormaterJournalAchat(filterModel, res.List);

                return new PagedComptabiliteList<JournalVenteModel>()
                {
                    List = listFormaterJournalVente,
                    PageNumber = res.PageNumber,
                    PageSize = res.PageSize,
                    TotalItems = res.TotalItems
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<IQueryable<JournalVenteSelectModel>> FilterJournalAchat(JournalVenteFilter filterModel)
        {
            try
            {

                if (filterModel.Periode == (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT)
                {
                    var periodeComptableActual = await _context.PeriodeComptables.OrderByDescending(x => x.DateDebut).Where(p => !p.DateCloture.HasValue).FirstOrDefaultAsync();

                    if (periodeComptableActual != null)
                    {
                        filterModel.DateMinimal = periodeComptableActual.DateDebut;
                        filterModel.DateMaximale = periodeComptableActual.DateDebut.AddMonths(periodeComptableActual.Periode).AddDays(-1);
                    }
                }

                IQueryable<JournalVenteSelectModel> query = _context.Depense.Where(x => x.Status != (int)StatutFacture.Brouillon)
                    .Select(depense => new JournalVenteSelectModel
                    {
                        Id = depense.Id,
                        DateCreation = depense.DateCreation,
                        Reference = depense.Reference,
                        NomClient = depense.Fournisseur.Nom,
                        CodeComptableClient = depense.Fournisseur.CodeComptable,
                        Articles = depense.Prestations,
                        TotalTTC = depense.Total,
                        Type = (int)TypeItemJournalAchat.DEPENSE,
                        TotalHT = depense.TotalHt,
                        TVA = depense.Tva,
                        IdFournisseur = depense.IdFournisseur,

                    })

                    .Where(x =>
                        (x.Reference.Contains(filterModel.SearchQuery)) &&
                        ((filterModel.Periode != (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT && filterModel.Periode != (int)PeriodeEnum.INTERVALLE) || filterModel.DateMinimal == null ? true : filterModel.DateMinimal <= x.DateCreation.Date) &&
                        ((filterModel.Periode != (int)PeriodeEnum.EXERCICE_COMPTABLE_COURANT && filterModel.Periode != (int)PeriodeEnum.INTERVALLE) || filterModel.DateMaximale == null ? true : filterModel.DateMaximale >= x.DateCreation.Date) &&
                        (filterModel.Periode == (int)PeriodeEnum.ANNEE_ACTUAL ? x.DateCreation.Date.Year == DateTime.Now.Year : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.ANNEE_DERNIERE ? x.DateCreation.Date.Year == (DateTime.Now.Year - 1) : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.MOIS_ACTUAL ? x.DateCreation.Date.Month == DateTime.Now.Month && x.DateCreation.Date.Year == DateTime.Now.Year : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.SIX_DERNIERE_MOIS ? (EntityExtensions.GetDateForMonth(-6) <= x.DateCreation.Date && EntityExtensions.GetDateForMonth(DateTime.Now.Month) >= x.DateCreation.Date) : true) &&
                        (filterModel.Periode == (int)PeriodeEnum.TROIS_DERNIERE_MOIS ? (EntityExtensions.GetDateForMonth(-3) <= x.DateCreation.Date && EntityExtensions.GetDateForMonth(DateTime.Now.Month) >= x.DateCreation.Date) : true)

                    )
                    .OrderBy(x => x.DateCreation);

                return query;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<List<JournalVenteModel>> FormaterJournalAchat(JournalVenteFilter filterModel, List<JournalVenteSelectModel> journalVentes)

        {
            try
            {
                var journalItems = new List<JournalVenteModel>();
                if (journalVentes.Count != 0)
                {
                    var categories = await _context.Categorie.ToListAsync();


                    foreach (var item in journalVentes)
                    {

                        List<PrestationsModule> prestations = JsonConvert.DeserializeObject<List<PrestationsModule>>(item.Articles);

                        var ListPrestations = prestations.Where(P => P.data.lotProduits == null).Select(e => e.data).ToList();

                        // var calculGenerale = calculGenerale(ListPrestations, item.Remise, item.TypeRemise);
                        var totalHtvv = totalHtDepense(ListPrestations, item.IdFournisseur);
                        var groupArticles = GroupeArticleByCategoriesDepense(ListPrestations, categories, item.IdFournisseur);

                        journalItems.Add(new JournalVenteModel()
                        {
                            CodeJournal = "ACHAT",
                            DateCreation = item.DateCreation,
                            NumeroCompte = item.CodeComptableClient,
                            NumeroPiece = item.Reference,
                            NomClient = item.NomClient,
                            Debit = item.TotalTTC.RoundingDouble(),
                            Credit = 0,

                        });

                        List<TvaModel> ListTva = JsonConvert.DeserializeObject<List<TvaModel>>(item.TVA);

                        foreach (var tvaA in ListTva)
                        {
                            var ListPlanComptable = _context.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.tvaModel)).FirstOrDefault();
                            List<PlanComptabeTvaModel> PlanComptabe = JsonConvert.DeserializeObject<List<PlanComptabeTvaModel>>(ListPlanComptable.Contenu);
                            string tva = System.Convert.ToString(tvaA.tva);
                            PlanComptabeTvaModel planComptabeTva = PlanComptabe.Where(plan => plan.tva == tva).FirstOrDefault();
                            double codeComptable = 0;
                            if (planComptabeTva == null)
                            {
                                codeComptable = PlanComptabe.Where(plan => plan.tva == "defaut").Select(x => x.codeComptable).FirstOrDefault();
                            }
                            else
                            {
                                codeComptable = planComptabeTva.codeComptable;
                            }
                            if (tvaA.totalTVA != 0)
                            {
                                journalItems.Add(new JournalVenteModel()
                                {
                                    CodeJournal = "ACHAT",
                                    DateCreation = item.DateCreation,
                                    NumeroCompte = codeComptable.ToString()/*tvas.racine + (tvas.tvalist.Where(t => t.valeurtva == tva.tva).FirstOrDefault()?.codecomptable ?? "")*/,
                                    NumeroPiece = item.Reference,
                                    NomClient = item.NomClient,
                                    Debit = 0,
                                    Credit = tvaA.totalTVA.RoundingDouble(),
                                });
                            }
                        }

                        foreach (var groupArticle in groupArticles)
                        {
                            //if(groupArticle.CodeComptable != null && groupArticle.Total !=0)
                            //{
                            journalItems.Add(new JournalVenteModel()
                            {
                                CodeJournal = "ACHAT",
                                DateCreation = item.DateCreation,
                                NumeroCompte = groupArticle.CodeComptable,
                                NumeroPiece = item.Reference,
                                NomClient = item.NomClient,
                                Debit = 0,
                                Credit = groupArticle.Total.RoundingDouble()
                            });
                            //}

                        }
                    }
                }
                
                return journalItems;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Groupe articles par categorie
        public List<GroupeArticle> GroupeArticleByCategoriesDepense(List<Data> articles, List<Entities.Categorie> categories, int? idFournisseur)
        {
            try
            {
                var groupeArticles = articles.GroupBy(x => x.categorie).Select(x => new GroupeArticle
                {

                    Total = totalHTArticleComptabilite(x.Sum(article => article.qte * article.prixParFournisseur.Where(y => y.idFournisseur == idFournisseur).Select(y => y.prix).SingleOrDefault()), 0, 0, null),
                    CodeComptable = categories.Where(cat => cat.Nom == x.FirstOrDefault().categorie).Select(p => p.Code_comptable).FirstOrDefault(),

                    //(string.IsNullOrEmpty(
                    //    categories.Where(cat => cat.Nom == x.FirstOrDefault().categorie).FirstOrDefault()?.Code_comptable) ? categories.Where(cat => cat.Nom == "Autre").FirstOrDefault().Code_comptable : categories.Where(cat => cat.Nom == x.FirstOrDefault().categorie).FirstOrDefault()?.Code_comptable)
                }).ToList();


                return groupeArticles;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<byte[]> ExportJournalVente(JournalVenteFilter filterModel)
        {
            try
            {
                var filterData = await FilterJournalVente(filterModel);
                var formaterData = await FormaterJournalVente(filterModel, filterData.ToList());
                return _generateEXCEL.GenerateJournalVenteFile(formaterData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<byte[]> ExportJournalAchat(JournalVenteFilter filterModel)
        {
            try
            {
                var filterData = await FilterJournalAchat(filterModel);
                var formaterData = await FormaterJournalAchat(filterModel, filterData.ToList());
                return _generateEXCEL.GenerateJournalAchatFile(formaterData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<byte[]> ExportComptabiliteComptes(ComptabiliteComptesFilter filterModel)
        {
            try
            {
                var filterData = await FilterComptabiliteComptes(filterModel);
                var formaterData = FormaterComptebiliteComptes(filterModel, filterData.ToList());
                return _generateEXCEL.GenerateJournalBanqueFile(formaterData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }
    }
}
