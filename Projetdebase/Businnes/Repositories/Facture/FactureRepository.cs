using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Repositories.Parametrage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Outils.Messagerie;

using ProjetBase.Businnes.Models.Paging;
using System.Linq.Expressions;
using ProjetBase.Businnes.Models.Sorting;
using InoMessagerie.Models;
using ProjetBase.Businnes.Repositories.Avoir;

namespace ProjetBase.Businnes.Repositories.Facture
{
    public class FactureRepository : EntityFrameworkRepository<Entities.Facture, int>, IFactureRepository
    {
        private readonly IParametrageRepository parametrageRepository;

        private readonly IAvoirRepository avoirRepository;

        public FactureRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            parametrageRepository = new ParametrageRepository(dbContext);

            avoirRepository = new AvoirRepository(dbContext);
        }

        //Sauvegarder les mémos de Facture
        public async Task<bool> SaveMemos(int id, string memos)
        {
            try
            {
                var facture = GetById(id);
                facture.Memos = memos;
                Update(facture);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }



        // Get statut à partir de son numéro
        public string getStatusFacture(int v)
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



        public async Task<Entities.Facture> GetFacture(int id)
        {
            try
            {
                var facture = await DbContext.Factures.Where(x => x.Id == id)
                                        .Include(x => x.Chantier).ThenInclude(x => x.Client)
                                        .Include(x => x.FicheInterventions)

                                        .Include(x => x.Devis)
                                         .Include(x => x.Avoirs)
                                        .Include(x => x.FacturePaiements).ThenInclude(x => x.Paiement).ThenInclude(x => x.ModeReglement)
                                        .Include(x => x.FacturePaiements).ThenInclude(x => x.Paiement).ThenInclude(x => x.ParametrageCompte)

                                    .FirstOrDefaultAsync();
                if (facture.IdChantier == null)
                {
                    facture.Client = DbContext.Clients.SingleOrDefault(X => X.Id == facture.IdClient);

                }
                return facture;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public List<Entities.Facture> GetFacturesChantier(int IdChantier, List<int> status, DateTime? DateDebut, DateTime? DateFin)
        {

            List<Entities.Facture> FacturesList = DbContext.Factures.Where(F =>
                                                            status.Contains(F.Status)
                                                            && F.IdChantier == IdChantier
                                                            && (DateDebut == null ? true : DateDebut <= F.DateCreation.Date)
                                                            && (DateFin == null ? true : DateFin >= F.DateCreation.Date)

                                                    )
                                                    .Include(x => x.Chantier).ThenInclude(x => x.Client)
                                                    .Include(x => x.FacturePaiements).ThenInclude(x => x.Paiement)
                                                    .ToList();


            return FacturesList;

        }

        // Annuler facture
        public async Task<object> AnnulerFacture(Entities.Facture facture, Entities.Avoir avoir)
        {
            try
            {
                AvoirModel parametreAvoir;

                var parametrageParDefaut = parametrageRepository.GetParametrageDocument();
                if (parametrageParDefaut.Type == (int)TypeParametrage.parametrageDocument)
                {
                    parametreAvoir = JsonConvert.DeserializeObject<AvoirModel>(parametrageParDefaut.Contenu);
                    avoir.DateCreation = DateTime.Now.Date;
                    avoir.DateEcheance = DateTime.Now.Date.AddDays(parametreAvoir.validite_avoir);
                }

                DbContext.Avoirs.Add(avoir);

                facture.Status = (int)StatutFacture.Annule;

                DbContext.Factures.Update(facture);

                await DbContext.SaveChangesAsync();

                return avoir;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }




        public async Task<MessagerResponseModel> SendEmail(int idFacture, InoMessagerie.Models.SendMailParamsModel MailParams)
        {
            try
            {
                Messagerie messagerie = new Messagerie(DbContext);

                MessagerResponseModel result = messagerie.SendEmail(MailParams);
                if (result.statut == 200)
                {
                    var currentUser = EntityExtensions.GetCurrentUser(DbContext);
                    Entities.Facture facture = DbContext.Factures.Where(F => F.Id == idFacture).FirstOrDefault();

                    if (facture.Emails != "" && facture.Emails != null)
                    {
                        var Emails = JsonConvert.DeserializeObject<List<object>>(facture.Emails);
                        foreach (var mailTo in MailParams.messageTo)
                        {
                            Emails.Add(new
                            {
                                email = mailTo,
                                idUser = currentUser.Id,
                                subject = MailParams.Subject,
                                content = MailParams.content,
                                date = DateTime.Now
                            });
                        }
                        facture.Emails = JsonConvert.SerializeObject(Emails);
                    }
                    else
                    {
                        facture.Emails = JsonConvert.SerializeObject(facture.Emails);
                    }


                    Update(facture);
                    await DbContext.SaveChangesAsync();

                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }



        public PagedList<Entities.Facture> Filter(PagingParams pagingParams, Expression<Func<Entities.Facture, bool>> filter = null, SortingParams sortingParams = null)
        {
            IQueryable<Entities.Facture> query = DbContext.Set<Entities.Facture>();
            query = query.Where(filter).Include(x => x.Chantier).ThenInclude(x => x.Client)
                    .Include(u => u.FacturePaiements)
                    .Select(f => new Entities.Facture()
                    {
                        Id = f.Id,
                        Reference = f.Reference,
                        Historique = f.Historique,
                        Memos = f.Memos,
                        Note = f.Note,
                        Object = f.Object,
                        Remise = f.Remise,
                        TypeRemise = f.TypeRemise,
                        Status = f.Status,
                        Total = f.Total,
                        TotalHt = f.TotalHt,
                        DateCreation = f.DateCreation,
                        DateEcheance = f.DateEcheance,
                        Prestations = f.Prestations,
                        Avoirs = f.Avoirs,
                        IdClient = f.IdClient,
                        Client = f.Client,

                        Comptabilise = f.Comptabilise,
                        Compteur = f.Compteur,
                        Emails = f.Emails,
                        UpdateAt = f.UpdateAt,
                        ConditionRegelement = f.ConditionRegelement,
                        FicheInterventions = f.FicheInterventions,

                        IdDevis = f.IdDevis,
                        FacturePaiements = f.FacturePaiements.Select(x => new FacturePaiement
                        {
                            Id = x.Id,
                            IdFacture = x.IdFacture,
                            IdPaiement = x.IdPaiement,
                            Montant = x.Montant,
                            Paiement = x.Paiement
                        }).ToList()
                    });

            if (sortingParams != null)
            {
                query = new SortedList<Entities.Facture>(query, sortingParams).GetSortedList();
            }

            return new PagedList<Entities.Facture>(query, pagingParams.PageNumber, pagingParams.PageSize);
        }

        public bool CheckUniqueReference(string reference)
        {
            var NbrReference = DbContext.Factures.Where(x => x.Reference == reference).Count();
            return NbrReference > 0;
        }

        public bool TransfertFichesInterventionToFacture(int idFacture, List<int> FichesInterventionIds)
        {
            try
            {
                var listFicheTravail = DbContext.FicheIntervention.Where(x => FichesInterventionIds.Contains(x.Id)).ToList();
                FichesInterventionIds.ForEach(idFicheTravail =>
                {
                    var FicheIntervention = listFicheTravail.Where(x => x.Id == idFicheTravail).FirstOrDefault();
                    FicheIntervention.Status = StatutFicheIntervention.Facturee;
                    FicheIntervention.IdFacture = idFacture;

                    DbContext.FicheIntervention.Update(FicheIntervention);
                });

                DbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {

            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();
                historiqueChamps.ForEach(h =>

                {
                    var dd = h.Attribute;
                    if (h.Attribute != "Prestations" && h.Attribute != "Historique" && h.Attribute != "Chantier" && h.Attribute != "typeFacture" && h.Attribute != "IdDevis" && h.Attribute != "Tva" && h.Attribute != "Historique" && h.Attribute != "UpdateAt" && h.Attribute != "DateCreation" && h.Attribute != "Memos" && h.Attribute != "Emails" && h.Attribute != "InfoClient")
                    {
                        if (h.Attribute == "IdChantier")
                        {
                            h.Attribute = "Chantier";
                            if (h.After != "")
                            {
                                int id = Convert.ToInt32(h.After);
                                h.After = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                            else
                            {
                                h.After = null;
                            }

                            if (h.Before != "")
                            {
                                int id = Convert.ToInt32(h.Before);
                                h.Before = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                            else
                            {
                                h.Before = null;
                            }
                        }

                        if (h.Attribute == "IdClient")
                        {
                            h.Attribute = "Client";
                            if (h.After != "")
                            {
                                int id = Convert.ToInt32(h.After);
                                h.After = DbContext.Clients.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                            if (h.Before != "")
                            {
                                int id = Convert.ToInt32(h.Before);
                                h.Before = DbContext.Clients.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                            }
                        }


                        if (h.Attribute == "Status")
                        {
                            h.After = getStatusFacture(Int32.Parse(h.After));
                            h.Before = getStatusFacture(Int32.Parse(h.Before));

                        }
                        if (h.Attribute == "Object")
                        {
                            h.Attribute = "Objet";
                        }
                        if (h.Attribute == "RetenueGarantie")
                        {
                            h.After = (h.After == "0" ? "Non" : "Oui");
                            h.Before = (h.Before == "0" ? "Non" : "Oui");
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

        async public Task<Entities.Facture> CreateFactureSituation(FactureSituationModal factureSituationModal)
        {
            try
            {
                #region ajouter la nouvelle facture
                DbContext.Factures.Add(factureSituationModal.facture);
                #endregion

                #region Obtenez le devis pour lequel nous voulons créer une facture de situation 
                Entities.Devis devis = DbContext.Devis.SingleOrDefault(D => D.Id == factureSituationModal.idDevis);
                #endregion

                await DbContext.SaveChangesAsync();

                #region changer l'id du facture dans la nouvelle situation
                factureSituationModal.situations.idFacture = factureSituationModal.facture.Id;
                #endregion

                #region l'ajout du nouvelle situation dans le devis
                if (devis.Situation == null)
                {
                    List<FactureSituationDevis> situation = new List<FactureSituationDevis>();
                    situation.Add(factureSituationModal.situations);
                    devis.Situation = JsonConvert.SerializeObject(situation);
                }
                else
                {
                    List<FactureSituationDevis> situation = JsonConvert.DeserializeObject<List<FactureSituationDevis>>(devis.Situation);
                    situation.Add(factureSituationModal.situations);
                    devis.Situation = JsonConvert.SerializeObject(situation);
                }
                devis.Status = StatutDevis.Acceptee;
                DbContext.Update(devis);
                #endregion

                if (devis.IdChantier == null)
                {
                    var chantier = DbContext.Chantier.SingleOrDefault(X => X.Id == devis.IdChantier);
                    chantier.Statut = chantier.Statut == Enum.StatutChantier.Termine ? Enum.StatutChantier.Accepte : chantier.Statut;
                    DbContext.Update(chantier);
                }

                await DbContext.SaveChangesAsync();
                return factureSituationModal.facture;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async public Task<Entities.Facture> CreateFactureAcompte(FactureAcomptesModal factureAcompteModal)
        {
            try
            {
                #region ajouter la nouvelle facture
                DbContext.Factures.Add(factureAcompteModal.facture);
                #endregion

                #region Obtenez le devis pour lequel nous voulons créer une facture de situation 
                Entities.Devis devis = DbContext.Devis.SingleOrDefault(D => D.Id == factureAcompteModal.idDevis);
                #endregion

                await DbContext.SaveChangesAsync();

                #region changer l'id du facture dans la nouvelle situation
                factureAcompteModal.Acomptes.idFacture = factureAcompteModal.facture.Id;
                #endregion

                #region l'ajout du nouvelle situation dans le devis
                if (devis.Acomptes == null || devis.Acomptes == "")
                {
                    List<FactureAcompteDevis> Acomptes = new List<FactureAcompteDevis>();
                    Acomptes.Add(factureAcompteModal.Acomptes);
                    devis.Acomptes = JsonConvert.SerializeObject(Acomptes);
                }
                else
                {
                    List<FactureAcompteDevis> Acomptes = JsonConvert.DeserializeObject<List<FactureAcompteDevis>>(devis.Acomptes);
                    Acomptes.Add(factureAcompteModal.Acomptes);
                    devis.Acomptes = JsonConvert.SerializeObject(Acomptes);
                }
                devis.Status = StatutDevis.Acceptee;
                DbContext.Update(devis);
                #endregion

                if (devis.IdChantier == null)
                {
                    var chantier = DbContext.Chantier.SingleOrDefault(X => X.Id == devis.IdChantier);
                    chantier.Statut = chantier.Statut == Enum.StatutChantier.Termine ? Enum.StatutChantier.Accepte : chantier.Statut;
                    DbContext.Update(chantier);
                }

                await DbContext.SaveChangesAsync();
                return factureAcompteModal.facture;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }

        public List<Entities.Facture> GetFacturesClient(int idClient, List<int> status, DateTime? DateDebut, DateTime? DateFin)
        {
            List<Entities.Facture> FacturesList = DbContext.Factures.Where(F =>
                                                           status.Contains(F.Status)
                                                           && F.IdClient == idClient
                                                           && (DateDebut == null ? true : DateDebut <= F.DateCreation.Date)
                                                           && (DateFin == null ? true : DateFin >= F.DateCreation.Date)

                                                   )
                                                   .Include(x => x.Chantier).ThenInclude(x => x.Client)
                                                   .Include(x => x.FacturePaiements).ThenInclude(x => x.Paiement)
                                                   .ToList();



            return FacturesList;


        }
    }
}
