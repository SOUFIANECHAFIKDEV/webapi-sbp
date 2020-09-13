using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using Newtonsoft.Json;
using ProjetBase.Businnes.Repositories.Parametrage;
using ProjetBase.Businnes.Repositories.Avoir;
using ProjetBase.Businnes.Repositories.Chantier;

namespace ProjetBase.Businnes.Repositories.Paiement
{
    public class PaiementRepository : EntityFrameworkRepository<Entities.Paiement, int>, IPaiementRepository
    {
        private readonly IParametrageRepository parametrageRepository;
        private readonly IAvoirRepository avoirRepository;
        private readonly IChantierRepository chantierRepository;


        public PaiementRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            parametrageRepository = new ParametrageRepository(dbContext);
            avoirRepository = new AvoirRepository(dbContext);
            chantierRepository = new ChantierRepository(dbContext);
        }

        // Ajouter un paiement
        public async Task<Entities.Paiement> Save(Entities.Paiement paiement)
        {
            try
            {
                // Pour crée un avoir
                if (paiement.Avoir != null)
                {
                    paiement.Avoir = await ConfigurerAvoirPaiement(paiement.Avoir);
                }

                // Pour changer staut avoir
                if (paiement.IdAvoir != null)
                {
                    var avoir = DbContext.Avoirs.Where(x => x.Id == paiement.IdAvoir).FirstOrDefault();
                    avoir.Status = (int)StatutAvoir.Utilise;
                    DbContext.Update(avoir);
                }



                Create(paiement);

                DbContext.SaveChanges();

                CheckFactureStatutAfterPaiement(paiement.FacturePaiements.Select(x => x.IdFacture).ToList());
                return paiement;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public void changeStatutToTermine(int idChantier)
        {
            try
            {
                var count = DbContext.Factures.Where(x =>
                                                        x.IdChantier == idChantier
                                                        && ((x.Status == (int)StatutFacture.Encours) || x.Status == (int)StatutFacture.Enretard)).Count();

                var cc = DbContext.Chantier.SingleOrDefault(x=> x.Id == idChantier);
                if (count == 0)
                {
                    if (cc.Statut != StatutChantier.Termine)
                    {
                        cc.Statut = StatutChantier.Termine;
                        DbContext.Update(cc);
                        DbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Historique paiement
        public List<ModifyEntryModel> Historique(Entities.Paiement paiement, Entities.Paiement paiementDB)
        {
            try
            {
                // Get differte between object send and stored object de paiement
                var champsModify = EntityExtensions.GetModification(paiementDB, paiement);

                champsModify.ForEach(h =>
            {
                if (h.Attribute != "Historique" && h.Attribute != "FacturePaiements" && h.Attribute != "UpdateAt" && h.Attribute != "Depense")
                {
                    if (h.Attribute == "IdCaisse")
                    {
                        h.Attribute = "Compte à créditer";
                        if (h.Before != "" && h.Before != null)
                        {
                            h.Before = DbContext.ParametrageCompte.Where(x => x.Id == int.Parse(h.Before)).FirstOrDefault().Nom;
                        }

                        if (h.After != "" && h.After != null)
                        {
                            h.After = DbContext.ParametrageCompte.Where(x => x.Id == int.Parse(h.After)).FirstOrDefault().Nom;
                        }
                    }
                    if (h.Attribute == "IdModePaiement")
                    {
                        h.Attribute = "Moyen de paiement";
                        if (h.Before != "" && h.Before != null)
                        {
                            h.Before = DbContext.ModeReglement.Where(x => x.Id == int.Parse(h.Before)).FirstOrDefault().Nom;
                        }

                        if (h.After != "" && h.After != null)
                        {
                            h.After = DbContext.ModeReglement.Where(x => x.Id == int.Parse(h.After)).FirstOrDefault().Nom;
                        }
                    }
                }


            });

                return champsModify;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Check statut facture aprés paiement 
        public void CheckFactureStatutAfterPaiement(List<int> idsFacture)
        {
            try
            {
                var factures = DbContext.Factures.Where(x => idsFacture.Contains(x.Id)).Include(x => x.FacturePaiements).ThenInclude(p => p.Paiement)
                    .Include(x => x.Devis).ThenInclude(x => x.Facture)
                    .ToList();

                foreach (var facture in factures)
                {
                    var factureSumPayement = facture.FacturePaiements.Sum(x => x.Montant);
                    if (facture.Total.RoundingDouble() == factureSumPayement.RoundingDouble())
                    {
                        facture.Status = (int)StatutFacture.Cloture;
                    }
                    else
                    {
                        if (facture.typeFacture != (int)TypeFacture.Generale)
                        {
                            var devis = facture.Devis;
                            var idFacture = devis.Facture.Where(F => F.typeFacture == facture.typeFacture).Max(x => x.Id);
                            if (facture.Id != idFacture  || (facture.Id == idFacture && facture.Status != (int)StatutFacture.Cloture))
                            {
                                //if (facture.DateEcheance.Date < DateTime.Now.Date)
                                //{
                                //    facture.Status = (int)StatutFacture.Enretard;
                                //}
                                //else
                                //{
                                //    facture.Status = (int)StatutFacture.Encours;
                                //}

                                facture.Status = (facture.DateEcheance.Date < DateTime.Now.Date) ? (facture.Status = (int)StatutFacture.Enretard) : (facture.Status = (int)StatutFacture.Encours);
                            }
                        }
                        else
                        {
                            facture.Status = (facture.DateEcheance.Date < DateTime.Now.Date) ? (facture.Status = (int)StatutFacture.Enretard) : (facture.Status = (int)StatutFacture.Encours);
                            //if (facture.DateEcheance.Date < DateTime.Now.Date)
                            //{
                            //    facture.Status = (int)StatutFacture.Enretard;
                            //}
                            //else
                            //{
                            //    facture.Status = (int)StatutFacture.Encours;
                            //}
                        }


                    }
                    facture.FacturePaiements = null;
                }


                DbContext.Factures.UpdateRange(factures);

                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Récpurer un paiement 
        public async Task<Entities.Paiement> GetPaiement(int id)
        {
            try
            {
                var paiement = await DbContext.Paiements.Where(x => x.Id == id)
                    .Include(x => x.Depense).ThenInclude(x => x.Chantier)
                    .Include(x => x.ModeReglement)
                    .Include(x => x.ParametrageCompte)
                    .Include(x => x.Avoir).ThenInclude(x => x.Chantier).ThenInclude(x => x.Client)
                    .Include(x => x.FacturePaiements).ThenInclude(x => x.Facture).ThenInclude(x => x.Chantier).ThenInclude(x => x.Client)

                    .FirstOrDefaultAsync();

                return paiement;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Supprimer un paiement 
        public void DeletePaiement(Entities.Paiement paiement)
        {
            try
            {
                var factureIds = paiement.FacturePaiements.Select(x => x.IdFacture).ToList();

                if (paiement.IdAvoir.HasValue)
                {
                    var avoir = paiement.Avoir;
                    avoir.Status = (int)StatutAvoir.Encours;
                    DbContext.Avoirs.Update(avoir);
                    paiement.Avoir = null;
                }

                //paiement.IsDeleted = true;

                //Update(paiement);
                Delete(paiement);

                DbContext.SaveChanges();

                CheckFactureStatutAfterPaiement(factureIds);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Modifier un paiement 
        public async Task UpdatePaiement(Entities.Paiement paiement)
        {
            try
            {
                Update(paiement);
                DbContext.FacturePaiements.UpdateRange(paiement.FacturePaiements);
                await DbContext.SaveChangesAsync();

                CheckFactureStatutAfterPaiement(paiement.FacturePaiements.Select(x => x.IdFacture).ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Movement compte à compte
        public async Task<bool> MovementCompteACompte(PaiementMovementCompteACompteModel paiementMovementCompte)
        {
            try
            {
                var currentUser = EntityExtensions.GetCurrentUser(DbContext);

                var debite = new Entities.Paiement();
                debite.Montant = -paiementMovementCompte.Montant;
                debite.type = (int)TypePaiement.VIREMENT_DEPUIS;
                debite.IdCaisse = paiementMovementCompte.CompteDebiter;

                debite.DatePaiement = paiementMovementCompte.datePaiement;
                debite.Comptabilise = (int)StatutComptabilise.Non;
                debite.IdModePaiement = paiementMovementCompte.idModePaiement;
                debite.Historique = paiementMovementCompte.historique;
                DbContext.Paiements.Add(debite);

                var crediter = new Entities.Paiement();
                crediter.Montant = paiementMovementCompte.Montant;
                crediter.type = (int)TypePaiement.VIREMENT_A;
                crediter.IdCaisse = paiementMovementCompte.CompteCrediter;

                crediter.DatePaiement = paiementMovementCompte.datePaiement;
                crediter.Comptabilise = (int)StatutComptabilise.Non;
                crediter.IdModePaiement = paiementMovementCompte.idModePaiement;
                crediter.Historique = paiementMovementCompte.historique;
                DbContext.Paiements.Add(crediter);

                var res = await DbContext.SaveChangesAsync();

                return res > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Get Solde courant
        public async Task<double> TotalPaiement(PaiementModel paiementModel)
        {
            try
            {
                var totaldep = await DbContext.Paiements.Where(x => (
                                (x.Description ?? "").Contains(paiementModel.SearchQuery)) &&
                                (paiementModel.DateDebut == null ? true : paiementModel.DateDebut <= x.DatePaiement.Date) &&
                                (paiementModel.DateFin == null ? true : paiementModel.DateFin >= x.DatePaiement.Date) &&
                                (paiementModel.IdCompte.HasValue ? x.IdCaisse == paiementModel.IdCompte : true) &&
                                (x.IdDepense != null)
                                ).SumAsync(x => x.Montant);
                var totalfac = await DbContext.Paiements.Where(x => (
                                (x.Description ?? "").Contains(paiementModel.SearchQuery)) &&
                                (paiementModel.DateDebut == null ? true : paiementModel.DateDebut <= x.DatePaiement.Date) &&
                                (paiementModel.DateFin == null ? true : paiementModel.DateFin >= x.DatePaiement.Date) &&
                                (paiementModel.IdCompte.HasValue ? x.IdCaisse == paiementModel.IdCompte : true) &&
                                (x.IdDepense == null)

                                ).SumAsync(x => x.Montant);
                var total = totalfac - totaldep;
                return total;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Configurer avoir crée par paiement
        public async Task<Entities.Avoir> ConfigurerAvoirPaiement(Entities.Avoir avoir)
        {
            try
            {
                var parametrage = DbContext.Parametrages.Where(x => x.Type == (int)TypeNumerotation.avoir).FirstOrDefault();

                // Get validité document avoir
                var parametrageParDefaut = DbContext.Parametrages.Where(x => x.Type == (int)TypeParametrage.parametrageDocument).FirstOrDefault();
                var parametreAvoir = JsonConvert.DeserializeObject<ParametrageDocument>(parametrageParDefaut.Contenu);
                avoir.DateEcheance = DateTime.Now.Date.AddDays(parametreAvoir.validite_avoir);
                avoir.DateCreation = DateTime.Now.Date;
                avoirRepository.CheckUniqueReference(avoir.Reference);
                IParametrageRepository parametrageRepository = new ParametrageRepository(DbContext);
                await parametrageRepository.Increment(TypeNumerotation.avoir);

                return avoir;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Check is valide paiement dans le cas de l'ajout
        public async Task<bool> CheckValidePaiementAjout(List<FacturePaiement> facturePaiements)
        {
            try
            {
                var idsFacture = facturePaiements.Select(x => x.IdFacture).ToList();
                var factures = await DbContext.Factures.Where(x => idsFacture.Contains(x.Id)).Include(x => x.FacturePaiements).ThenInclude(x => x.Paiement).ToListAsync();
                foreach (var facturePaiement in facturePaiements)
                {
                    var facture = factures.Where(x => x.Id == facturePaiement.IdFacture).FirstOrDefault();
                    var sumInDb = facture.FacturePaiements.Sum(x => x.Montant);
                    //if (facture.Status == (int)StatutFacture.Cloture)
                    //{
                    //    return false;
                    //}
                    if ((sumInDb + facturePaiement.Montant).RoundingDouble() > facture.Total.RoundingDouble())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Check is valide paiement dans le cas de modification 
        public async Task<bool> CheckValidePaimentEdit(List<FacturePaiement> facturePaiements)
        {
            try
            {
                var idFacture = facturePaiements.Select(x => x.IdFacture).FirstOrDefault();
                var facture = await DbContext.Factures.Where(x => idFacture == x.Id).Include(x => x.FacturePaiements).ThenInclude(x => x.Paiement).FirstOrDefaultAsync();
                var facturePaiement = facture.FacturePaiements.ToList();

                var paiementFactureModifier = facturePaiements.FirstOrDefault();

                for (var i = 0; i < facturePaiement.Count(); i++)
                {
                    if (facturePaiement[i].Id == paiementFactureModifier.Id)
                    {
                        facturePaiement[i] = paiementFactureModifier;
                    }
                }

                var val1 = facturePaiement.Sum(x => x.Montant).RoundingDouble();
                var val2 = facture.Total.RoundingDouble();

                if (val1 <= val2)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public void DeletePaiementDepense(Entities.Paiement paiement)
        {
            try
            {
                if (paiement.IdAvoir.HasValue)
                {
                    var avoir = paiement.Avoir;
                    avoir.Status = (int)StatutAvoir.Encours;
                    DbContext.Avoirs.Update(avoir);
                    paiement.Avoir = null;
                }

                Delete(paiement);
                DbContext.SaveChanges();

                CheckDepenseStatutAfterPaiement(paiement.IdDepense);


            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        // Check statut facture aprés paiement 
        public void CheckDepenseStatutAfterPaiement(int? idsFacture)
        {
            try
            {
                var depenses = DbContext.Depense.Where(x => x.Id == idsFacture);

                var factureSumPayement = DbContext.Paiements.Where(x => x.IdDepense == idsFacture).Sum(x => x.Montant);
                foreach (var depense in depenses)
                {
                    if (depense.Total.RoundingDouble() == factureSumPayement.RoundingDouble())
                    {
                        depense.Status = (int)StatutFacture.Cloture;
                    }
                    else
                    {
                        if (depense.DateExpiration.Date < DateTime.Now.Date)
                        {
                            depense.Status = (int)StatutFacture.Enretard;
                        }
                        else
                        {
                            depense.Status = (int)StatutFacture.Encours;
                        }
                    }
                    depense.Paiements = null;
                    DbContext.Depense.Update(depense);
                }



                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        async public Task<Entities.Paiement> SavePaiementDepense(Entities.Paiement paiement)
        {
            try
            {
                // Pour crée un avoir
                if (paiement.Avoir != null)
                {
                    paiement.Avoir = await ConfigurerAvoirPaiement(paiement.Avoir);
                }

                // Pour changer staut avoir
                if (paiement.IdAvoir != null)
                {
                    var avoir = DbContext.Avoirs.Where(x => x.Id == paiement.IdAvoir).FirstOrDefault();
                    avoir.Status = (int)StatutAvoir.Utilise;
                    DbContext.Update(avoir);
                }

                Create(paiement);

                DbContext.SaveChanges();

                CheckDepenseStatutAfterPaiement(paiement.IdDepense);

                return paiement;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        async public Task UpdatePaiementDepense(Entities.Paiement paiement)
        {
            try
            {
                Update(paiement);
                //DbContext.Paiements.UpdateRange(paiement);
                await DbContext.SaveChangesAsync();

                CheckDepenseStatutAfterPaiement(paiement.IdDepense);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        async public Task<bool> CheckValidePaimentDepenseEdit(Entities.Paiement paiements)
        {
            try
            {
                //var depenses = DbContext.Depense.Where(x => x.Id == Paiements.);
                // var idFacture = paiements.Select(x => x.).FirstOrDefault();
                var IdDepense = paiements.IdDepense;
                var facture = await DbContext.Depense.Where(x => IdDepense == x.Id).Include(x => x.Paiements).FirstOrDefaultAsync();
                var facturePaiement = facture.Paiements.ToList();

                var paiementFactureModifier = paiements;

                for (var i = 0; i < facturePaiement.Count(); i++)
                {
                    if (facturePaiement[i].Id == paiementFactureModifier.Id)
                    {
                        facturePaiement[i] = paiementFactureModifier;
                    }
                }

                var val1 = facturePaiement.Sum(x => x.Montant).RoundingDouble();
                var val2 = facture.Total.RoundingDouble();

                if (val1 <= val2)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        async public Task<bool> CheckValidePaiementDepenseAjout(Entities.Paiement paiements)
        {
            try
            {
                var IdDepense = paiements.IdDepense;
                var depenses = await DbContext.Depense.Where(x => IdDepense == x.Id).Include(x => x.Paiements).ToListAsync();

                //  var factures = await DbContext.De.Where(x => idsFacture.Contains(x.Id)).Include(x => x.FacturePaiements).ThenInclude(x => x.Paiement).ToListAsync();

                var facture = depenses.Where(x => x.Id == IdDepense).FirstOrDefault();
                var sumInDb = facture.Paiements.Sum(x => x.Montant);
                if (facture.Status == (int)StatutFacture.Cloture)
                {
                    return false;
                }
                else if ((sumInDb + paiements.Montant).RoundingDouble() > facture.Total.RoundingDouble())
                {
                    return false;
                }

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
                    if (h.Attribute != "Historique" && h.Attribute != "FacturePaiements" && h.Attribute != "UpdateAt" && h.Attribute != "Depense")
                    {
                        {
                            if (h.Attribute == "IdCaisse")
                            {
                                h.Attribute = "Compte à créditer";
                                if (h.Before != "" && h.Before != null)
                                {
                                    h.Before = DbContext.ParametrageCompte.Where(x => x.Id == int.Parse(h.Before)).FirstOrDefault().Nom;
                                }

                                if (h.After != "" && h.After != null)
                                {
                                    h.After = DbContext.ParametrageCompte.Where(x => x.Id == int.Parse(h.After)).FirstOrDefault().Nom;
                                }
                            }
                            if (h.Attribute == "IdModePaiement")
                            {
                                h.Attribute = "Moyen de paiement";
                                if (h.Before != "" && h.Before != null)
                                {
                                    h.Before = DbContext.ModeReglement.Where(x => x.Id == int.Parse(h.Before)).FirstOrDefault().Nom;
                                }

                                if (h.After != "" && h.After != null)
                                {
                                    h.After = DbContext.ModeReglement.Where(x => x.Id == int.Parse(h.After)).FirstOrDefault().Nom;
                                }
                            }
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
    }
}
