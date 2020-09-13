using Microsoft.EntityFrameworkCore;
using Nager.Date;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Chantier
{
    public class ChantierRepository : EntityFrameworkRepository<Entities.Chantier, int>, IChantierRepository
    {
        public ChantierRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            dbContext = dbContext;
        }

        public bool CheckUniqueName(string nom)
        {
            var NbrReference = DbContext.Chantier.Where(x => x.Nom == nom).Count();
            return NbrReference > 0;
        }

        public Task<List<Entities.Chantier>> GetAllChantier()
        {
            try
            {
                var chantiers = DbContext.Chantier
                    .Include(x => x.Client)
                    .ToListAsync();

                return chantiers;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<Entities.Chantier> GetChantier(int id)
        {
            try
            {
                var chantier = await DbContext.Chantier.Where(x => x.Id == id)
                                   .Include(x => x.Client)
                                   .FirstOrDefaultAsync();
                return chantier;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public bool supprimerChantier(int id)
        {
            try
            {
                var Chantier = DbContext.Chantier.SingleOrDefault(C => C.Id == id);
                //vérifier que chantier
                if (DbContext.Factures.Where(PF => PF.IdChantier == id).Count() != 0)
                {
                    return false;
                }

                if (DbContext.Avoirs.Where(PF => PF.IdChantier == id).Count() != 0)
                {
                    return false;
                }

                if (DbContext.Depense.Where(PF => PF.IdChantier == id).Count() != 0)
                {
                    return false;
                }

                if (DbContext.Devis.Where(PF => PF.IdChantier == id).Count() != 0)
                {
                    return false;
                }

                if (DbContext.FicheIntervention.Where(PF => PF.IdChantier == id).Count() != 0)
                {
                    return false;
                }

                if (DbContext.BonCommandeFournisseur.Where(PF => PF.IdChantier == id).Count() != 0)
                {
                    return false;
                }

                DbContext.Remove(Chantier);
                DbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)

            {
                Log.Error(ex, ex.Message);

                return false;
            }

        }

        Models.ChantierChangeStatusResponse IChantierRepository.changeStatut(ChangeStatutBodyRequest bodyRequest)
        {
            Entities.Chantier chantierInDb = DbContext.Chantier.SingleOrDefault(x => x.Id == bodyRequest.idChantier);

            #region verifier si le chantier a des facture spas encore payée
            if (bodyRequest.statutChantier == StatutChantier.Termine)
            {
                var count = DbContext.Factures
                    .Where(x =>
                    x.IdChantier == bodyRequest.idChantier
                    && (x.Status == (int)StatutFacture.Encours) || x.Status == (int)StatutFacture.Enretard).Count();
                if (count != 0)
                {
                    return new Models.ChantierChangeStatusResponse
                    {
                        result = Enum.ChantierChangeStatusResponse.has_bills_not_paid,
                        chantier = chantierInDb
                    };
                }
            }
            #endregion

            chantierInDb.Statut = bodyRequest.statutChantier;
            DbContext.Update(chantierInDb);

            if (DbContext.SaveChanges() > 0)
            {
                return new Models.ChantierChangeStatusResponse
                {
                    result = Enum.ChantierChangeStatusResponse.changed_successfully,
                    chantier = chantierInDb
                };
            }
            else
            {
                return new Models.ChantierChangeStatusResponse
                {
                    result = Enum.ChantierChangeStatusResponse.server_error,
                    chantier = chantierInDb
                };
            }
        }
        public Models.nbDocumentsChantieModel nbDocuments(int idChantier)
        {
            return DbContext.Chantier.Include(D => D.Devis)
                            .Include(F => F.Factures)
                            .Include(BC => BC.BonCommandeFournisseur)
                            .Where(X => X.Id == idChantier)
                            .Select(C => new nbDocumentsChantieModel { nbBC = C.BonCommandeFournisseur.Count(), nbFacture = C.Factures.Count(), nbDevis = C.Devis.Count() })
                            .FirstOrDefault();
        }
        
        Entities.Chantier IChantierRepository.getChantier(int id)
        {
            try
            {
                var chantier = DbContext.Chantier.Where(x => x.Id == id)
                          .Include(x => x.Client)
                          .Include(x => x.Factures)
                          .Include(x => x.Devis)
                          .Include(x => x.BonCommandeFournisseur)
                          .FirstOrDefault();
                return chantier;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }
        public RecapitulatifFinancierModel GetRecapitulatifFinancier(int idChantier)
        {
            var chantier = DbContext.Chantier.Include(x => x.Devis)
                  .Include(x => x.Factures).ThenInclude(x => x.FacturePaiements).ThenInclude(x => x.Paiement)
                  .Include(x => x.Depense).ThenInclude(x => x.Paiements)
                  .Include(x => x.FicheIntervention)
                  .Where(x => x.Id == idChantier)
                  .Select(x => new Entities.Chantier
                  {
                      Id = x.Id,
                      Nom = x.Nom,
                      Description = x.Description,
                      Commentaire = x.Commentaire,
                      Date_creation = x.Date_creation,
                      Statut = x.Statut,
                      IdClient = x.IdClient,
                      //Client = x.Client,
                      Montant = x.Montant,
                      NombrHeure = x.NombrHeure,
                      Historique = x.Historique,
                      Depense = x.Depense.Where(d => d.Status != (int)StatutDepense.Annule && d.Status != (int)StatutDepense.Brouillon)
                      .Select(d => new Entities.Depense()
                      {
                          categorie = d.categorie,
                          Status = d.Status,
                          Total = d.Total,
                          TotalHt = d.TotalHt,
                          Reference = d.Reference,
                          Paiements = d.Paiements.Select(p => new Entities.Paiement
                          {
                              Id = p.Id,
                              IdCaisse = p.IdCaisse,
                              IdDepense = p.IdDepense,
                              DatePaiement = p.DatePaiement,
                              Montant = p.Montant,
                              IdModePaiement = p.IdModePaiement,
                              Depense = p.Depense,
                          }).ToList()
                      }
                      )

                      .ToList(),
                      Devis = x.Devis.Where(d => d.Status != StatutDevis.Annulee && d.Status != StatutDevis.NonAcceptee).ToList(),
                      Factures = x.Factures.Where(d => d.Status != (int)StatutFacture.Annule && d.Status != (int)StatutFacture.Brouillon).

                      Select(f => new Entities.Facture()
                      {
                          Id = f.Id,
                          Reference = f.Reference,
                          Prestations = f.Prestations,
                          Status = f.Status,
                          Total = f.Total,
                          TotalHt = f.TotalHt,
                          IdDevis = f.IdDevis,
                          Devis = f.Devis,
                          DateCreation = f.DateCreation,
                          DateEcheance = f.DateEcheance,
                          RetenueGarantie = f.RetenueGarantie,
                          typeFacture = f.typeFacture,
                       
                          FacturePaiements = f.FacturePaiements.Select(fp => new FacturePaiement
                          {
                              Id = fp.Id,
                              IdFacture = fp.IdFacture,
                              IdPaiement = fp.IdPaiement,
                              Montant = fp.Montant,
                              Paiement = fp.Paiement
                          }).ToList()
                      }).
                      ToList(),
                      FicheIntervention = x.FicheIntervention.Where(fc => fc.Status == StatutFicheIntervention.Realisee || fc.Status == StatutFicheIntervention.Facturee).ToList()
                  })
                  .FirstOrDefault();
            var previsionnel = getprevisionnel(chantier);
            var facturation_tresorie = getFacturationTresorieModel(chantier, previsionnel);
            double tresorerieChantiessr = 0;
             tresorerieChantiessr = tresorerieChantier(chantier);
            var res = new RecapitulatifFinancierModel
            {
                previsionnel = previsionnel,
                facturation_tresorie = facturation_tresorie,
                tresorerieChantier = tresorerieChantiessr,

            };
            return res;

        }

        public List<FacturationTresorieModel> getFacturationTresorieModel(Entities.Chantier chantier, List<PrevisionnelElementsModel> previsionnel)
        {
            List<FacturationTresorieModel> facturationTresorie = new List<FacturationTresorieModel>();
            // total facture
            var facturePayes = GetFacturePayes(chantier.Factures.ToList());
            var factureRestAPayes = GetFactureRestPayes(chantier.Factures.ToList(), facturePayes.sum);
            var totalFacture = facturePayes.sum + factureRestAPayes.sum;
            double? sumTotalDevis = previsionnel.Where(x => x.typeElements == TypePrevisionnel.total_devis).Select(x => x.sum).FirstOrDefault();
            double? PourcentageCAFacture = 0;
            if (sumTotalDevis != 0)
            {
                PourcentageCAFacture = calculMargeDefferent(totalFacture, sumTotalDevis) ;
            }

            facturationTresorie.Add(new FacturationTresorieModel
            {
                typeElements = typeFactureTresorie.caFacture,

                sum = totalFacture,
                elements = new List<ElementsFacturationTresorieModel>() {
                 facturePayes,
                 factureRestAPayes,
                },
                defference = PourcentageCAFacture,
                pourcentage = null,
            });

            var depenseAchat = chantier.Depense.Where(x => x.categorie == (int)categorieDepense.achat).ToList();
            var depenseSousTraitance = chantier.Depense.Where(x => x.categorie == (int)categorieDepense.soustraitance).ToList();
            var depenseAchatMateriel = GetDepenseAchatMateriels(depenseAchat, previsionnel);
            var depenseEngageesSousTraitance = GetDepenseEngageesSousTraitance(depenseSousTraitance, previsionnel);
            var ficheIntervention = GetDepenseEngageesIntervention(chantier.FicheIntervention.ToList(), previsionnel);
            double? sumDepenseEngagees = depenseAchatMateriel.sum + depenseEngageesSousTraitance.sum + ficheIntervention.sum;
            double? sumTotalMargePrevisionnel = previsionnel.Where(x => x.typeElements == TypePrevisionnel.marge_Previsionnel).Select(x => x.sum).FirstOrDefault();
            facturationTresorie.Add(new FacturationTresorieModel
            {
                typeElements = typeFactureTresorie.depenseeEngagees,

                sum = sumDepenseEngagees,
                elements = new List<ElementsFacturationTresorieModel>() {
                 depenseAchatMateriel,
                ficheIntervention,
                 depenseEngageesSousTraitance
                },
                pourcentage = null,
            });
            var margeReeleMaterielle = GetMargeReelleMateriel(chantier.Factures.ToList(), depenseAchatMateriel.sum);
            var margeReelleMainOeuvre = GetMargeReelleMainOeuvre(chantier.Factures.ToList(), depenseEngageesSousTraitance.sum);

            double? sumMargeReelle = margeReeleMaterielle.sum + margeReelleMainOeuvre.sum;
            var margeReelleRetenueGarentie = GetRetenueGarantieMargeReelle(chantier.Factures.ToList(), sumMargeReelle);
            facturationTresorie.Add(new FacturationTresorieModel
            {
                typeElements = typeFactureTresorie.margeReele,

                sum = sumMargeReelle,
                elements = new List<ElementsFacturationTresorieModel>() {
                margeReelleRetenueGarentie,
                margeReeleMaterielle,
                 margeReelleMainOeuvre
                },
                defference = calculMargeDefferent(sumMargeReelle, sumTotalMargePrevisionnel) ,
                pourcentage = calculMargeGaint(sumDepenseEngagees, totalFacture) ,
                
            });
            return facturationTresorie;
        }
        public List<PrevisionnelElementsModel> getprevisionnel(Entities.Chantier chantier)
        {
            List<PrevisionnelElementsModel> previsionnel = new List<PrevisionnelElementsModel>();
            // total devis
            var coutVenteMateriel = GetCoutVenteMateriel(chantier.Devis.ToList());
            var coutVenteMainOveure = GetVenteMainOveure(chantier.Devis.ToList());
            double? sumprevisionnelTotalDevis = coutVenteMateriel.sum + coutVenteMainOveure.sum;

            previsionnel.Add(new PrevisionnelElementsModel
            {
                typeElements = TypePrevisionnel.total_devis,
                sum = sumprevisionnelTotalDevis,
                elements = new List<ElementsPrevisionnelModel>() {
                   coutVenteMateriel ,
                   coutVenteMainOveure,
                },
                pourcentage = null,
            });
            // depense a prevoir
            var coutAchatMateriel = GetAchatCoutMateriel(chantier.Devis.ToList());
            var coutAchatMainOveure = GetAchatCoutMainOeuvre(chantier.Devis.ToList());
            var coutSousTraitance = GetAchatSousTraitant(chantier);
            double? sumprevisionnelDepensePrevoir = coutAchatMateriel.sum + coutAchatMainOveure.sum + coutSousTraitance.sum;
            previsionnel.Add(new PrevisionnelElementsModel
            {
                typeElements = TypePrevisionnel.depensee_Aprevoir,
                sum = sumprevisionnelDepensePrevoir,
                elements = new List<ElementsPrevisionnelModel>() {
                   coutAchatMateriel,
                   coutAchatMainOveure,
                   coutSousTraitance,
                },
                pourcentage = null,
            });
            // Marge Previsionnel
            var sumPrevisionnel = sumprevisionnelTotalDevis - (sumprevisionnelDepensePrevoir);
            var retenueGarantie = GetRetenueGarantiePrevisionnel(chantier.Devis.ToList(), sumPrevisionnel);
            var MergePrevisionnelMateriel = GetMergeMaterielPrevisionnel(coutVenteMateriel.sum, coutAchatMateriel.sum, sumPrevisionnel);
            var coutAchatMainOeuvre = coutAchatMainOveure.sum + coutSousTraitance.sum;
            var MergePrevisionnelMainOeuvre = GetMergeMainOeuvre(coutVenteMainOveure.sum, coutAchatMainOeuvre);
            previsionnel.Add(new PrevisionnelElementsModel
            {
                typeElements = TypePrevisionnel.marge_Previsionnel,
                sum = sumPrevisionnel,
                elements = new List<ElementsPrevisionnelModel>() {
                   retenueGarantie,
                   MergePrevisionnelMateriel,
                   MergePrevisionnelMainOeuvre,

                },
                pourcentage = calculMargeGaint(sumprevisionnelDepensePrevoir,sumprevisionnelTotalDevis )

            });
            return previsionnel;
        }
        public ElementsPrevisionnelModel GetCoutVenteMateriel(List<Entities.Devis> devis)
        {
            var prestations = devis.Where(x=>x.Prestation != null).Select(x => x.Prestation).ToList();
            
            double sumLotProduit = 0;
            double sumP = 0;
            foreach (var prestation in prestations)
            {
                List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(prestation);
                sumP += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => x.data.cout_materiel * x.data.qte);
                var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                   .Select(x => x.data.lotProduits).ToList();
                foreach (var produitInLot in ListLot)
                {
                    sumLotProduit += produitInLot.Select(x => x.idProduitNavigation.cout_materiel * x.idProduitNavigation.qte).Sum();
                }
            }
            double devisMinimaliste = devis.Where(x => x.Prestation ==null).Select(x=>x.CoutMateriel).Sum();
            double sum = sumP + sumLotProduit + devisMinimaliste;
            return new ElementsPrevisionnelModel
            {
                type = (int)total_devis.vente_materiel,
                sum = sum,
                NbrHeure = null,
                pourcentage = null,
                souselements = null,
            };


        }

        public ElementsPrevisionnelModel GetRetenueGarantiePrevisionnel(List<Entities.Devis> devis, double? sumMargePrevisionnel)
        {
            double? sum = devis.Where(D => D.RetenueGarantie != null).Sum(D => D.TotalHt * (D.RetenueGarantie / 100));


            return new ElementsPrevisionnelModel
            {
                type = (int)MargePrevisionnel.retenueGarantier,
                sum = sum,
                NbrHeure = null,
                pourcentage = calculMargeDefferent(sum, sumMargePrevisionnel),
                souselements = null,
            };


        }
        public ElementsPrevisionnelModel GetVenteMainOveure(List<Entities.Devis> devis)
        {
            var prestations = devis.Where(x=>x.Prestation != null).Select(x => x.Prestation).ToList();
            double sumLotProduit = 0;
            double sumP = 0;
            double NbrHeure = 0;
            foreach (var prestation in prestations)
            {
                List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(prestation);
                sumP += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => (x.data.cout_vente * x.data.nomber_heure) * x.data.qte);
                NbrHeure += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => x.data.nomber_heure);
                var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                   .Select(x => x.data.lotProduits).ToList();
                foreach (var produitInLot in ListLot)
                {
                    sumLotProduit += produitInLot.Select(x => (x.idProduitNavigation.cout_vente * x.idProduitNavigation.nomber_heure) * x.idProduitNavigation.qte).Sum();
                    NbrHeure += produitInLot.Select(x => x.idProduitNavigation.nomber_heure).Sum();
                }
            }
           var devisMinimaliste =  devis.Where(x => x.Prestation == null).ToList();
            double sumdevisMinimaliste = 0;
            foreach (var devism in devisMinimaliste)
            {
                sumdevisMinimaliste += devism.NomberHeure * devism.CoutVente;
                NbrHeure += devism.NomberHeure;
            }
            double sum = sumP + sumLotProduit + sumdevisMinimaliste;
            double nombreh = NbrHeure;
            return new ElementsPrevisionnelModel
            {
                type = (int)total_devis.vente_main_oveure,
                sum = sum,
                NbrHeure = nombreh,
                pourcentage = null,
                souselements = null,
            };

        }

        public ElementsPrevisionnelModel GetAchatCoutMateriel(List<Entities.Devis> devis)
        {
            var prestations = devis.Where(x=>x.Prestation != null).Select(x => x.Prestation).ToList();
            double sumLotProduit = 0;
            double sumP = 0;
            foreach (var prestation in prestations)
            {
                List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(prestation);
                sumP += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Select(x =>
                      (x.data.prixParFournisseur.SingleOrDefault(f => f.@default == 1)).prix * x.data.qte
                  ).Sum();
                var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                   .Select(x => x.data.lotProduits).ToList();
                foreach (var produitInLot in ListLot)
                {
                    sumLotProduit += produitInLot.Select(x =>
                      (x.idProduitNavigation.prixParFournisseur.SingleOrDefault(f => f.@default == 1)).prix * x.idProduitNavigation.qte
                  ).Sum();
                }
            }
            double sumdevisMinimaliste = 0;
            var devisMinimaliste = devis.Where(x => x.Prestation == null).ToList();

            foreach (var devism in devisMinimaliste)
            {
                sumdevisMinimaliste += devism.AchatMateriel;
           
            }
            return new ElementsPrevisionnelModel
            {
                type = (int)depenseAprevoir.achat_materiel,
                sum = sumP + sumLotProduit + sumdevisMinimaliste,
                NbrHeure = null,
                pourcentage = null,
                souselements = null,
            };
        }

        public ElementsPrevisionnelModel GetAchatCoutMainOeuvre(List<Entities.Devis> devis)
        {
            var couHoraire = DbContext.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.coutsHoraire)).FirstOrDefault();
            var cout = JsonConvert.DeserializeObject<coutsHoraireModel>(couHoraire.Contenu);
            double CoutAchat = cout.prixAchat;
            var prestations = devis.Where(x => x.Prestation != null).Select(x => x.Prestation).ToList();
            double sumLotProduit = 0;
            double sumP = 0;
            double NbrHeure = 0;
            foreach (var prestation in prestations)
            {
            
                List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(prestation);
                sumP += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => (x.data.nomber_heure * CoutAchat) * x.data.qte);
                NbrHeure += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => x.data.nomber_heure);
                var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                   .Select(x => x.data.lotProduits).ToList();
                foreach (var produitInLot in ListLot)
                {
                    sumLotProduit += produitInLot.Select(x => (CoutAchat * x.idProduitNavigation.nomber_heure) * x.idProduitNavigation.qte).Sum();
                    NbrHeure += produitInLot.Select(x => x.idProduitNavigation.nomber_heure).Sum();
                }
            }
            double sumdevisMinimaliste = 0;
            var devisMinimaliste = devis.Where(x => x.Prestation == null ).ToList();

            foreach (var devism in devisMinimaliste)
            {
                sumdevisMinimaliste += devism.NomberHeure * CoutAchat;
                NbrHeure += devism.NomberHeure;
            }
           // double sum = sumP + sumLotProduit + sumdevisMinimaliste ;
            double sumPrestations = sumP + sumLotProduit + sumdevisMinimaliste;
            return new ElementsPrevisionnelModel
            {
                type = (int)depenseAprevoir.achat_main_oveure,
                sum = sumPrestations,
                NbrHeure = NbrHeure,
                pourcentage = null,
                souselements = null,
            };
        }

        public ElementsPrevisionnelModel GetAchatSousTraitant(Entities.Chantier chantier)
        {
            return new ElementsPrevisionnelModel
            {
                type = (int)depenseAprevoir.sous_traitance,
                sum =  chantier.Montant,
                NbrHeure = chantier.NombrHeure,
                pourcentage = null,
                souselements = null,
            };
        }

        public ElementsPrevisionnelModel GetMergeMaterielPrevisionnel(double? sumVenteMateriel, double? sumAchatMateriel, double? sumPrevisionnel)
        {
            return new ElementsPrevisionnelModel
            {
                type = (int)MargePrevisionnel.margeMateriel,
                sum = sumVenteMateriel - sumAchatMateriel,
                NbrHeure = null,
                pourcentage = calculMargeGaint(sumAchatMateriel, sumVenteMateriel) /*((sumVenteMateriel - sumAchatMateriel) / sumPrevisionnel) * 100*/,
                souselements = null,
            };
        }

        public ElementsPrevisionnelModel GetMergeMainOeuvre(double? sumVenteMainOeuvre, double? sumAchatMainOeuvre)
        {

            double? sum = sumVenteMainOeuvre - sumAchatMainOeuvre;
            return new ElementsPrevisionnelModel
            {
                type = (int)MargePrevisionnel.margeMainOeuvre,
                sum = sum,
                NbrHeure = null,
                pourcentage = calculMargeGaint(sumAchatMainOeuvre,sumVenteMainOeuvre),
                souselements = null,
            };
        }
        public ElementsFacturationTresorieModel GetFacturePayes(List<Entities.Facture> factures)
        {
            double? factureSumPayement = 0;
            factureSumPayement = factures.Where(x => x.FacturePaiements != null).Select(x => x.FacturePaiements.Sum(fp => fp.Montant)).Sum();
            return new ElementsFacturationTresorieModel
            {
                type = (int)caFacture.payes,
                sum = factureSumPayement,
                NbrHeure = null,
                pourcentage = null,
                souselements = null,
                defference = null,
            };
        }

        public ElementsFacturationTresorieModel GetFactureRestPayes(List<Entities.Facture> factures, double? totalePaiement)
        {
            double? sumResteAPayes = factures.Sum(x => x.Total);
            return new ElementsFacturationTresorieModel
            {
                type = (int)caFacture.enAttentepaiement,
                sum = sumResteAPayes - totalePaiement,
                NbrHeure = null,
                pourcentage = null,
                souselements = null,
                defference = null,
            };
        }

        public ElementsFacturationTresorieModel GetDepenseAchatMateriels(List<Entities.Depense> depenses, List<PrevisionnelElementsModel> previsionnel)
        {

            double? depenseSumPayement = 0;
            double? sumTotalDepense = 0;
            double? sumCoutAchatMateriel = 0;
            double? depEnseEnAttentepaiement = 0;

            depenseSumPayement = depenses.Where(x => x.Paiements != null).Select(x => x.Paiements.Sum(p => p.Montant)).Sum();
            sumTotalDepense = depenses.Sum(x => x.Total);

            sumCoutAchatMateriel = previsionnel.Where(x => x.typeElements == TypePrevisionnel.depensee_Aprevoir).Select(x => x.elements.Where(d => d.type == (int)depenseAprevoir.achat_materiel).Select(d => d.sum).FirstOrDefault()).FirstOrDefault();
            depEnseEnAttentepaiement = sumTotalDepense - depenseSumPayement;
            var sousElements = new List<SousElementsFacturationTresorieModel>()
            {
                new SousElementsFacturationTresorieModel() {
                    sum = depenseSumPayement,
                    typeSousElement = (int)depenseAchat.payes,
                    pourcentage =   calculMargeDefferent(depenseSumPayement , sumTotalDepense) ,
                    NbrPanierOrDeplacement = null,
                },

                  new SousElementsFacturationTresorieModel() {
                    sum = depEnseEnAttentepaiement,
                    typeSousElement = (int)depenseAchat.enAttentepaiement,
                    pourcentage = calculMargeDefferent(depEnseEnAttentepaiement , sumTotalDepense),
                       NbrPanierOrDeplacement = null,
                },
            };
            return new ElementsFacturationTresorieModel
            {
                type = (int)depenseEngagees.achatsMateriels,
                sum = sumTotalDepense,
                NbrHeure = null,
                pourcentage = null,
                souselements = sousElements,
                defference = calculMargeDefferent(sumTotalDepense, sumCoutAchatMateriel),

            };
        }
        public ElementsFacturationTresorieModel GetDepenseEngageesSousTraitance(List<Entities.Depense> depenses, List<PrevisionnelElementsModel> previsionnel)
        {

            double depenseSumPayement = 0;
            double sumTotalDepense = 0;
            double fsumTotalDepense = 0;
            depenseSumPayement = depenses.Where(x => x.Paiements != null).Select(x => x.Paiements.Sum(p => p.Montant)).Sum();
            sumTotalDepense = depenses.Sum(x => x.Total);
            var sumCoutAchatMateriel = previsionnel.Where(x => x.typeElements == TypePrevisionnel.depensee_Aprevoir).Select(x => x.elements.Where(d => d.type == (int)depenseAprevoir.sous_traitance).Select(d => d.sum).FirstOrDefault()).FirstOrDefault();
            double depEnseEnAttentepaiement = sumTotalDepense - depenseSumPayement;

            var sousElements = new List<SousElementsFacturationTresorieModel>()
            {
                new SousElementsFacturationTresorieModel() {
                    sum = depenseSumPayement,
                    typeSousElement = (int)depenseSousTraitent.payes,
                    pourcentage =calculMargeDefferent(depenseSumPayement,sumTotalDepense),
                    NbrPanierOrDeplacement = null,
                },

                  new SousElementsFacturationTresorieModel() {
                    sum = depEnseEnAttentepaiement,
                    typeSousElement = (int)depenseSousTraitent.enAttentepaiement,
                    pourcentage = calculMargeDefferent(depEnseEnAttentepaiement , sumTotalDepense) ,
                    NbrPanierOrDeplacement = null,
                },
            };
            return new ElementsFacturationTresorieModel
            {
                type = (int)depenseEngagees.sousTraitance,
                sum = sumTotalDepense,
                NbrHeure = null,
                pourcentage = null,
                defference =   calculMargeDefferent(sumTotalDepense, sumCoutAchatMateriel),
                souselements = sousElements,

            };
        }

        public ElementsFacturationTresorieModel GetRetenueGarantieMargeReelle(List<Entities.Facture> factures, double? sumMargeReelle)
        {
            double? sum = factures.Where(D => D.RetenueGarantie != null).Sum(D => D.TotalHt * (D.RetenueGarantie / 100));


            return new ElementsFacturationTresorieModel
            {
                type = (int)MargeReelle.retenueGarantier,
                sum = sum,
                NbrHeure = null,
                pourcentage = calculMargeDefferent(sum, sumMargeReelle),
                souselements = null,
            };
        }
        public ElementsFacturationTresorieModel GetMargeReelleMateriel(List<Entities.Facture> Factures, double? sumAchatMateriel)
        {

            var prestations = Factures.Where(x=>x.typeFacture == (int)TypeFacture.Generale).Select(x => x.Prestations).ToList();
            var FactureAcomptesSituations = Factures.Where(x => x.typeFacture != (int)TypeFacture.Generale && x.IdDevis !=null).ToList();

            double sumLotProduit = 0;
            double sumP = 0;
            foreach (var prestation in prestations)
            {
                List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(prestation);
                sumP += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => x.data.cout_materiel * x.data.qte);
                var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                   .Select(x => x.data.lotProduits).ToList();
                foreach (var produitInLot in ListLot)
                {
                    sumLotProduit += produitInLot.Select(x => x.idProduitNavigation.cout_materiel * x.idProduitNavigation.qte).Sum();
                }
            }

            double coutMaterielFactureAcomptesSituations = 0;
            foreach(var factureacompte in FactureAcomptesSituations)
            {
                coutMaterielFactureAcomptesSituations += venteMaterielFactureAcomptesSituations(factureacompte);

            }

            double? sumCoutVenteMaterielFacture = sumP + sumLotProduit + coutMaterielFactureAcomptesSituations;
            double? totalsumCoutMateriel = sumCoutVenteMaterielFacture + sumAchatMateriel;
            double? sumMargeRelleMateriele = sumCoutVenteMaterielFacture - sumAchatMateriel;
            return new ElementsFacturationTresorieModel
            {
                type = (int)MargeReelle.margeMateriel,
                sum = sumMargeRelleMateriele,
                NbrHeure = null,
                pourcentage = calculMargeGaint(sumAchatMateriel, sumCoutVenteMaterielFacture), /*calculPourcentage(sumMargeRelleMateriele, totalsumCoutMateriel)*/
                souselements = null,

            };
        }
        public double? calculMargeDefferent(double? nbr1, double? nbr2)
        {
            //var result = nbr1 != 0 && nbr2 != 0 ? (((nbr1 - nbr2) * 100) / nbr1) : 0;
            var result = (nbr1 != 0 && nbr2 != 0) ? ((nbr1) / (nbr2)) * 100 : 0;
            return result;
        }
        public double? calculMargeGaint(double? prixachat, double? prixvente)
        {
            //return (prixvente == 0 && prixachat == 0) ? 0 : (((prixvente + prixachat) / (prixvente - prixachat)) * 100);
            return prixachat == 0 ? 100 : (((prixvente - prixachat) * 100) / prixachat);
        }
        public ElementsFacturationTresorieModel GetMargeReelleMainOeuvre(List<Entities.Facture> Factures, double? sumAchatMainOeuvre)
        {

            var prestations = Factures.Select(x => x.Prestations).ToList();
            var FactureAcomptesSituations = Factures.Where(x => x.typeFacture != (int)TypeFacture.Generale).ToList();
            double sumLotProduit = 0;
            double sumP = 0;
            double NbrHeure = 0;
            foreach (var prestation in prestations)
            {
                List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(prestation);
                sumP += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => (x.data.cout_vente * x.data.nomber_heure) * x.data.qte);
                NbrHeure += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => x.data.nomber_heure);
                var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                   .Select(x => x.data.lotProduits).ToList();
                foreach (var produitInLot in ListLot)
                {
                    sumLotProduit += produitInLot.Select(x => (x.idProduitNavigation.cout_vente * x.idProduitNavigation.nomber_heure) * x.idProduitNavigation.qte).Sum();
                    NbrHeure += produitInLot.Select(x => x.idProduitNavigation.nomber_heure).Sum();
                }
            }
            double coutMainOeuvreFactureAcomptesSituations = 0;
            foreach (var factureacompte in FactureAcomptesSituations)
            {
                coutMainOeuvreFactureAcomptesSituations += ventecoutMainOeuvreFactureAcomptesSituations(factureacompte);

            }
            double sumVenteMainOeuvreFacture = sumP + sumLotProduit + coutMainOeuvreFactureAcomptesSituations;
            double? sumMainOeuvre = sumVenteMainOeuvreFacture + sumAchatMainOeuvre;
            return new ElementsFacturationTresorieModel
            {
                type = (int)MargeReelle.margeMainOeuvre,
                sum = sumVenteMainOeuvreFacture - sumAchatMainOeuvre,
                NbrHeure = null,
                pourcentage = calculMargeGaint(sumAchatMainOeuvre, sumVenteMainOeuvreFacture),/*calculPourcentage((sumVenteMainOeuvreFacture - sumAchatMainOeuvre) , sumMainOeuvre)*/
                souselements = null,

            };
        }
        public double tresorerieChantier(Entities.Chantier chantier)
        {
            double factureSumPayement = 0;
            double depenseSumPayement = 0;
            factureSumPayement = chantier.Factures.Where(x => x.FacturePaiements != null).Select(x => x.FacturePaiements.Sum(d => d.Montant)).Sum();
            depenseSumPayement = chantier.Depense.Where(x => x.Paiements != null).Select(x => x.Paiements.Sum(d => d.Montant)).Sum();
            double sumtresorerieChantier = factureSumPayement - depenseSumPayement;
            return sumtresorerieChantier;

        }

        public ElementsFacturationTresorieModel GetDepenseEngageesIntervention(List<Entities.FicheIntervention> interventions, List<PrevisionnelElementsModel> previsionnel)
        {
            var couHoraire = DbContext.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.coutsHoraire)).FirstOrDefault();
            var cout = JsonConvert.DeserializeObject<coutsHoraireModel>(couHoraire.Contenu);
            var horaireTravail = DbContext.Parametrages.SingleOrDefault(p => p.Type == (int)TypeParametrage.horaireTravail);
            double nbrHeure = interventions.Select(x => GetInterventionNbHours(x, horaireTravail)).Sum();
            var sumCoutAchatMateriel = previsionnel.Where(x => x.typeElements == TypePrevisionnel.depensee_Aprevoir).Select(x => x.elements.Where(d => d.type == (int)depenseAprevoir.achat_main_oveure).Select(d => d.sum).FirstOrDefault()).FirstOrDefault();
            double sumPanier = interventions.Select(x => x.NombrePanier * cout.coutPanier).Sum();
            double sumDeplacement = interventions.Select(x => x.NombreDeplacement * cout.coutDeplacement).Sum();
            double? nombrePanier = interventions.Select(x => x.NombrePanier).Sum();
            double? nombreDeplacement = interventions.Select(x => x.NombreDeplacement).Sum();
            var sousElements = new List<SousElementsFacturationTresorieModel>()
            {
                new SousElementsFacturationTresorieModel() {
                    sum =sumPanier ,
                    typeSousElement = (int)InterventionsousElement.panier,
                    pourcentage =null,
                    NbrPanierOrDeplacement = nombrePanier,
                },

                  new SousElementsFacturationTresorieModel() {
                    sum = sumDeplacement,
                    typeSousElement = (int)InterventionsousElement.deplacement,
                    pourcentage = null,
                    NbrPanierOrDeplacement = nombreDeplacement,
                },
            };
            double sumIntervention = nbrHeure * cout.prixAchat + sumPanier + sumDeplacement;
            return new ElementsFacturationTresorieModel
            {
                type = (int)depenseEngagees.interventions,
                sum = sumIntervention,
                NbrHeure = nbrHeure,
                pourcentage = null,
                defference = calculMargeDefferent(sumIntervention,sumCoutAchatMateriel),
                souselements = sousElements,

            };
        }

        public double GetInterventionNbHours(Entities.FicheIntervention FI, Entities.Parametrages horaireTravailConfig)
        {
            try
            {
                //le nomber total des heurs d'intervention
                double FiNbTotalHeurs = 0;

                //calculer le nomber des jours fériés
                var publicHolidays = DateSystem.GetPublicHoliday(CountryCode.FR, FI.DateDebut, FI.DateFin).Where(x => x.Global);

                //calculer le nomber des jour des week-end
                var weekendDates = FI.DateDebut.GetWeekEnd(FI.DateFin);

                //retire les jour des weekend a partir des jour fériér
                var holiDays = publicHolidays.Where(x => !weekendDates.Any(y => x.Date.Date == y.Date)).ToList();

                //la somme des jours non consedirable dans le calcule
                var countDays = weekendDates.Count + holiDays.Count;

                //le nomber des jours consedirable
                var days = 0;

                //la somme des jours consedirable dans le calcule
                days = (int)((FI.DateFin - FI.DateDebut).TotalDays) - countDays;

                //vérifier si le nomber des heures d'intervention et supérieur a zero
                if (days >= 0)
                {
                    //récupér le pagametrage d'horaire de tavaille
                    //var horaireTravailConfig = DbContext.Parametrages.SingleOrDefault(p => p.Type == (int)TypeParametrage.horaireTravail);

                    //selectionnée les heures de taravaille
                    var horaireTravail = JsonConvert.DeserializeObject<ParametrageHoraireTravail>(horaireTravailConfig.Contenu);

                    //convertir l'heur de debut du travaille

                    DateTime debut = DateTime.ParseExact(horaireTravail.heureDebut, "hh:mm", System.Globalization.CultureInfo.CurrentCulture);

                    //convertir l'heur du fin du travaille
                    DateTime fin = DateTime.ParseExact(horaireTravail.heureFin, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);

                    //calculer le nomber des heures du travaille 
                    var nbHoraireTravailheurs = (fin - debut).TotalHours;

                    //calculer le nomber des heurs total d'interventions
                    FiNbTotalHeurs = nbHoraireTravailheurs * days;
                }

                //retourné le nomber total des heurs d'intervention
                return FiNbTotalHeurs;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public List<RetenueGarantieModel> GetRetenueGarantie(int idChantier)
        {
            var chantier = DbContext.Chantier.Include(x => x.Factures).ThenInclude(x => x.FacturePaiements).ThenInclude(x => x.Paiement)
                .Where(x => x.Id == idChantier)
                  .Select(x => new Entities.Chantier
                  {
                      Id = x.Id,
                      Nom = x.Nom,
                      Description = x.Description,
                      Commentaire = x.Commentaire,
                      Date_creation = x.Date_creation,
                      Statut = x.Statut,
                      IdClient = x.IdClient,
                      Montant = x.Montant,
                      NombrHeure = x.NombrHeure,
                      Historique = x.Historique,
                      Factures = x.Factures.Where(d => d.Status != (int)StatutFacture.Annule && d.Status != (int)StatutFacture.Brouillon).
                      Select(f => new Entities.Facture()
                      {
                          Id = f.Id,
                          Reference = f.Reference,
                          Prestations = f.Prestations,
                          Status = f.Status,
                          Total = f.Total,
                          TotalHt = f.TotalHt,
                          DateCreation = f.DateCreation,
                          DateEcheance = f.DateEcheance,
                          RetenueGarantie = f.RetenueGarantie,
                          DelaiGarantie = f.DelaiGarantie,
                          StatusGarantie=f.StatusGarantie,
                          Avoirs = f.Avoirs,
                          Chantier = f.Chantier,
                          ConditionRegelement = f.ConditionRegelement,
                          FacturePaiements = f.FacturePaiements.Select(fp => new FacturePaiement
                          {
                              Id = fp.Id,
                              IdFacture = fp.IdFacture,
                              IdPaiement = fp.IdPaiement,
                              Montant = fp.Montant,
                              Paiement = fp.Paiement
                          }).ToList()
                      }).
                      ToList(),
                  })
                  .FirstOrDefault();

            var ListFacture = chantier.Factures.Where(x => x.RetenueGarantie != null).ToList();
            List<RetenueGarantieModel> ListRetenue = new List<RetenueGarantieModel>();
          
            foreach (var facture in ListFacture)
            {
                var retenue = new RetenueGarantieModel();
                retenue.idFacture = facture.Id;
                retenue.reference = facture.Reference;
                retenue.valeurRetenue = facture.RetenueGarantie;
                retenue.dateEcheanceRetenue = getdateEcheanceRetenue(facture);
                retenue.htRetenus = facture.TotalHt * (facture.RetenueGarantie / 100);
                retenue.statutRetenue = getdateEcheanceRetenue(facture) ==null ? facture.StatusGarantie : getStatutRetenu(getdateEcheanceRetenue(facture), facture.Id);
                ListRetenue.Add(retenue);
            }
          

            return ListRetenue;
        }
        public int?  getStatutRetenu(DateTime? date,int idFacture)
        {
            var facture = DbContext.Factures.Where(x => x.Id == idFacture).Include(x => x.FacturePaiements).ThenInclude(p => p.Paiement)
                  .Include(x => x.Devis).ThenInclude(x => x.Facture)
                  .FirstOrDefault();

            if (facture.StatusGarantie == (int)StatutRetenueGarantie.encours || facture.StatusGarantie == (int)StatutRetenueGarantie.enretard)
            {
                facture.StatusGarantie = (date?.Date < DateTime.Now.Date) ? (facture.StatusGarantie = (int)StatutRetenueGarantie.enretard) : (facture.StatusGarantie = (int)StatutRetenueGarantie.encours);
            }
            DbContext.Update(facture);
            DbContext.SaveChanges();
            return facture.StatusGarantie;
        }

        public DateTime? getdateEcheanceRetenue(Entities.Facture facture)
        {
            if (facture.Status == (int)StatutFacture.Cloture)
            {
                var ListFacturePaiment = facture.FacturePaiements;
                var facturePaiment = ListFacturePaiment.OrderBy(t => t.Paiement.DatePaiement)
                                    .FirstOrDefault();
                var DelaiGarantie = Convert.ToInt32(facture.DelaiGarantie);
                var Date = facturePaiment.Paiement.DatePaiement.AddMonths(DelaiGarantie);
                return Date;
            }
            else { return null;}

        }


        public double venteMaterielFactureAcomptesSituations(Entities.Facture facture)
        {
            try
            {
                double coutmateriel = 0;
                var pourcentageFacture = (facture.TotalHt / facture.Devis.TotalHt) * 100;
                if (facture.Devis.Prestation != null)
                {
                    List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(facture.Devis.Prestation);
                    coutmateriel += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => (x.data.cout_materiel * x.data.qte) * (pourcentageFacture / 100));
                    var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                    .Select(x => x.data.lotProduits).ToList();
                    foreach (var produitInLot in ListLot)
                    {
                        coutmateriel += produitInLot.Select(x => (x.idProduitNavigation.cout_materiel * x.idProduitNavigation.qte) * (pourcentageFacture / 100)).Sum();
                    }
                }
                else
                {
                    coutmateriel = facture.Devis.CoutMateriel * (pourcentageFacture / 100);

                }
                return coutmateriel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public double ventecoutMainOeuvreFactureAcomptesSituations(Entities.Facture facture)
        {
            try
            {
                double coutmainOeuvre = 0;
                var pourcentageFacture = (facture.TotalHt / facture.Devis.TotalHt) * 100;
                if (facture.Devis.Prestation != null)
                {
                    List<PrestationsModule> listPrestation = JsonConvert.DeserializeObject<List<PrestationsModule>>(facture.Devis.Prestation);
                    coutmainOeuvre += listPrestation.Where(x => x.type == (int)typePrestation.Produit).Sum(x => (x.data.nomber_heure * x.data.qte * x.data.cout_vente) * (pourcentageFacture / 100));
                    var ListLot = listPrestation.Where(x => x.type == (int)typePrestation.Lot)
                 .Select(x => x.data.lotProduits).ToList();
                    foreach (var produitInLot in ListLot)
                    {
                        coutmainOeuvre += produitInLot.Select(x => (x.idProduitNavigation.nomber_heure * x.idProduitNavigation.qte * x.idProduitNavigation.cout_vente) * (pourcentageFacture / 100)).Sum();
                    }
                }
                else
                {
                    coutmainOeuvre = (facture.Devis.NomberHeure * facture.Devis.CoutVente) * (pourcentageFacture / 100);

                }

                return coutmainOeuvre;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}





