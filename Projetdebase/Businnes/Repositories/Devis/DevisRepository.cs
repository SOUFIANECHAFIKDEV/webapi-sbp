using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using Serilog;

using ProjetBase.Businnes.Outils.Messagerie;
using ProjetBase.Businnes.Extensions;
using Newtonsoft.Json;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Repositories.Chantier;
using System.Collections;
using ProjetBase.Businnes.Models.Filter;
using ProjetBase.Businnes.Repositories.Parametrage;
using InoAuthentification.UserManagers;

namespace ProjetBase.Businnes.Repositories.Devis
{
    public class DevisRepository : EntityFrameworkRepository<Entities.Devis, int>, IDevisRepository
    {
        private readonly IChantierRepository chantierRepository;
        public ProjetBaseContext SbpContext;
        private readonly IParametrageRepository parametrageRepository;
        public DevisRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            SbpContext = dbContext;
            chantierRepository = new ChantierRepository(dbContext);
            parametrageRepository = new ParametrageRepository(dbContext);
        }

        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();
                historiqueChamps.ForEach(h =>
                
                {
                    var cc = h.Attribute;
                    if (h.Attribute != "Id" && h.Attribute != "Historique" && h.Attribute != "Tva"  && h.Attribute != "Chantier" && h.Attribute != "Prestation"  /*&& h.Attribute != "AdresseFacturation" && h.Attribute != "AdresseIntervention"*/ && h.Attribute != "Memos" && h.Attribute != "Emails" && h.Attribute != "BonCommandeFournisseur" && h.Attribute != "Facture")
                    {
                        if (h.Attribute == "RetenueGarantie")
                        {
                            h.Attribute = "Retenue de Garantie";
                            h.After = (h.After == "0" ? "Non" : "Oui");
                            h.Before = (h.Before == "0" ? "Non" : "Oui");
                        }
                        if (h.Attribute == "ConditionReglement")
                        {
                            h.Attribute = "Condition de Réglement";
                            
                        }
                        if (h.Attribute == "TypeRemise")
                        {
                            h.Attribute = "Type Remise";

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
                        if (h.Attribute == "IdChantier")
                        {
                            h.Attribute = "Chantier";
                            int id = Convert.ToInt32(h.After);
                            h.After = SbpContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;

                            id = Convert.ToInt32(h.Before);
                            h.Before = SbpContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                        }
                        if (h.Attribute == "Status")
                        {

                            h.After = getLabelStatut(h.After);
                            h.Before = getLabelStatut(h.Before);
                        }
                        if (h.Attribute == "AdresseIntervention")
                        {
                            h.Attribute = "Adresse Intervention";
                            if (h.Before != "" && h.Before != null)
                            {
                                var AdresseIntervention = JsonConvert.DeserializeObject<AdresseModel>(h.Before);
                                var adressedesignation = AdresseIntervention.designation;
                                var adressecomplementAdresse = AdresseIntervention.complementAdresse;
                                var adressedepartement = AdresseIntervention.departement;
                                var adressecodePostal = AdresseIntervention.codePostal;
                                var adresseadresse = AdresseIntervention.adresse;
                                var adresseville = AdresseIntervention.ville;
                                // var adressepays = AdresseIntervention.pays;
                                var htmladressedesignation = "<strong> Désignation : </strong>" + adressedesignation + "</br>";
                                var htmladressedresse = "<strong>  Complément Adresset : </strong>" + adresseadresse + "</br>";
                                var htmladressecomplementAdresse = "<strong>  Adresse  : </strong>" + adressecomplementAdresse + "</br>";
                                var htmladresseville = "<strong>  Ville : </strong>" + adresseville + "</br>";
                                var htmladressedepartement = "<strong>  Département : </strong>" + adressedepartement + "</br>";
                                var htmladressecodepostal = "<strong>  Code Postal : </strong>" + adressecodePostal + "</br>";
                                //var htmladressepays = "<strong>  Code Postal : </strong>" + adressepays + "</br>";
                                h.Before = htmladressedesignation + htmladressedresse + htmladressecomplementAdresse + htmladresseville + htmladressecodepostal + htmladressedepartement;

                            }

                            if (h.After != "" && h.After != null)
                            {

                                var AdresseIntervention = JsonConvert.DeserializeObject<AdresseModel>(h.After);
                                var adressedesignation = AdresseIntervention.designation;
                                var adressecomplementAdresse = AdresseIntervention.complementAdresse;
                                var adressedepartement = AdresseIntervention.departement;
                                var adressecodePostal = AdresseIntervention.codePostal;
                                var adresseadresse = AdresseIntervention.adresse;
                                var adresseville = AdresseIntervention.ville;
                                // var adressepays = AdresseIntervention.pays;
                                var htmladressedesignation = "<strong> Désignation : </strong>" + adressedesignation + "</br>";
                                var htmladressedresse = "<strong>  Complément Adresse : </strong>" + adresseadresse + "</br>";
                                var htmladressecomplementAdresse = "<strong>  Adresse  : </strong>" + adressecomplementAdresse + "</br>";
                                var htmladresseville = "<strong>  Ville : </strong>" + adresseville + "</br>";
                                var htmladressedepartement = "<strong>  Département : </strong>" + adressedepartement + "</br>";
                                var htmladressecodepostal = "<strong>  Code Postal : </strong>" + adressecodePostal + "</br>";
                                //var htmladressepays = "<strong>  Code Postal : </strong>" + adressepays + "</br>";
                                h.After = htmladressedesignation + htmladressedresse + htmladressecomplementAdresse + htmladresseville + htmladressecodepostal + htmladressedepartement;

                            }
                         
                        }

                        if (h.Attribute == "AdresseFacturation")
                        {
                            h.Attribute = "Adresse Facturation";
                            if (h.Before != "" && h.Before != null)
                            {
                                var AdresseIntervention = JsonConvert.DeserializeObject<AdresseModel>(h.Before);
                                var adressedesignation = AdresseIntervention.designation;
                                var adressecomplementAdresse = AdresseIntervention.complementAdresse;
                                var adressedepartement = AdresseIntervention.departement;
                                var adressecodePostal = AdresseIntervention.codePostal;
                                var adresseadresse = AdresseIntervention.adresse;
                                var adresseville = AdresseIntervention.ville;
                                // var adressepays = AdresseIntervention.pays;
                                var htmladressedesignation = "<strong> Désignation : </strong>" + adressedesignation + "</br>";
                                var htmladressedresse = "<strong>  Complément Adresset : </strong>" + adresseadresse + "</br>";
                                var htmladressecomplementAdresse = "<strong>  Adresse  : </strong>" + adressecomplementAdresse + "</br>";
                                var htmladresseville = "<strong>  Ville : </strong>" + adresseville + "</br>";
                                var htmladressedepartement = "<strong>  Département : </strong>" + adressedepartement + "</br>";
                                var htmladressecodepostal = "<strong>  Code Postal : </strong>" + adressecodePostal + "</br>";
                                //var htmladressepays = "<strong>  Code Postal : </strong>" + adressepays + "</br>";
                                h.Before = htmladressedesignation + htmladressedresse + htmladressecomplementAdresse + htmladresseville + htmladressecodepostal + htmladressedepartement;

                            }

                            if (h.After != "" && h.After != null)
                            {

                                var AdresseIntervention = JsonConvert.DeserializeObject<AdresseModel>(h.After);
                                var adressedesignation = AdresseIntervention.designation;
                                var adressecomplementAdresse = AdresseIntervention.complementAdresse;
                                var adressedepartement = AdresseIntervention.departement;
                                var adressecodePostal = AdresseIntervention.codePostal;
                                var adresseadresse = AdresseIntervention.adresse;
                                var adresseville = AdresseIntervention.ville;
                                // var adressepays = AdresseIntervention.pays;
                                var htmladressedesignation = "<strong> Désignation : </strong>" + adressedesignation + "</br>";
                                var htmladressedresse = "<strong>  Complément Adresse : </strong>" + adresseadresse + "</br>";
                                var htmladressecomplementAdresse = "<strong>  Adresse  : </strong>" + adressecomplementAdresse + "</br>";
                                var htmladresseville = "<strong>  Ville : </strong>" + adresseville + "</br>";
                                var htmladressedepartement = "<strong>  Département : </strong>" + adressedepartement + "</br>";
                                var htmladressecodepostal = "<strong>  Code Postal : </strong>" + adressecodePostal + "</br>";
                                //var htmladressepays = "<strong>  Code Postal : </strong>" + adressepays + "</br>";
                                h.After = htmladressedesignation + htmladressedresse + htmladressecomplementAdresse + htmladresseville + htmladressecodepostal + htmladressedepartement;

                            }
                        }

                        if (h.Attribute == "NomberHeure")
                        {
                            h.Attribute = "Nombre d'heure";

                        }

                        if (h.Attribute == "CoutVente")
                        {
                            h.Attribute = "coût vente";

                        }

                        if (h.Attribute == "CoutMateriel")
                        {
                            h.Attribute = "Coût matériel";

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

        private string getLabelStatut(string statut)
        {
            switch (statut)
            {
                case "EnAttente":
                    return "En Attente";
                case "Acceptee":
                    return "Accepté";
                case "NonAcceptee":
                    return "Refusé";
                case "Annulee":
                    return "Annulé";
                case "Facture":
                    return "Facturé";
                default:
                    return "";
            }
        }

        async public Task<bool> changerStatutChantier(Entities.Devis devis)
        {
            //DbContext
            try
            {
                if (devis.Status == StatutDevis.Acceptee)
                {
                    Entities.Chantier chantier = DbContext.Chantier.Where(C => C.Id == devis.IdChantier).FirstOrDefault();
                    if (chantier.Statut == StatutChantier.Enetude)
                    {
                        //chantier.TauxAvancement = chantier.TauxAvancement;
                        chantier.Statut = StatutChantier.Accepte;
                        chantierRepository.Update(chantier);
                        int result = await DbContext.SaveChangesAsync();
                        return result > 0 ? true : false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);

                throw;
            }
        }

        public bool CheckUniqueReference(string reference)
        {
            var NbrReference = DbContext.Devis.Where(x => x.Reference == reference).Count();
            return NbrReference > 0;
        }

        public Entities.Devis GetDevis(int id)
        {
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
            }
        }

        public async Task<MessagerResponseModel> SendEmail(int idDevis, InoMessagerie.Models.SendMailParamsModel MailParams)
        {
            try
            {
                Messagerie messagerie = new Messagerie(DbContext);

                MessagerResponseModel result = messagerie.SendEmail(MailParams);
                if (result.statut == 200)
                {
                    var currentUser = EntityExtensions.GetCurrentUser(DbContext);
                    Entities.Devis devis = DbContext.Devis.Where(F => F.Id == idDevis).FirstOrDefault();
                    if (devis.Emails != "" && devis.Emails != null)
                    {
                        var Emails = JsonConvert.DeserializeObject<List<object>>(devis.Emails);
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
                        devis.Emails = JsonConvert.SerializeObject(Emails);
                    }
                    else
                    {
                        devis.Emails = JsonConvert.SerializeObject(devis.Emails);
                    }


                    Update(devis);
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

        public Entities.BonCommandeFournisseur createBCObjet(Entities.Devis devis, int idfournis, List<Data> prestation)
        {
            try
            {

                var prestations = new PrestationsModule();
                List<PrestationsModule> ListPrestation = new List<PrestationsModule>();

                foreach (var u in prestation)
                {

                    prestations = new PrestationsModule
                    {
                        data = u,
                        qte = u.qte,
                        type = 0,
                        remise = 0,

                    };
                    ListPrestation.Add(prestations);
                }
             
                double totalHt = 0;
                double totalTtc = 0;
                var list = new List<double>();
                var listTtc = new List<double>();
                dynamic listprestations;

                var TvaList = new List<TvaModel>();
                Entities.BonCommandeFournisseur bc = new Entities.BonCommandeFournisseur();
                bc.Articles = JsonConvert.SerializeObject(ListPrestation);
                //list des Tvas
                foreach (var cc in prestation)
                {

                    double? prix = cc.prixParFournisseur.Where(x=>x.@default == 1).Select(x=>x.prix).FirstOrDefault();
                    double? qte = cc.qte;
                    totalHt =(double) (prix * qte);
                    totalTtc = totalHt + (totalHt * cc.tva) / 100;
                    list.Add(totalHt);
                    listTtc.Add(totalTtc);
                    TvaList.Add(new TvaModel()
                    {tva = cc.tva,
                    totalHT = totalHt,
                    totalTTC = totalTtc,
                    totalTVA = (totalHt * cc.tva) / 100,
                    });


                }
           
               //groupe Tva
                var result =   TvaList.GroupBy(entry => entry.tva).Select(g => new { tva = g.Key, totalHT = g.Sum(e => e.totalHT), totalTTC = g.Sum(e => e.totalTTC), totalTVA = g.Sum(e => e.totalTVA) });
                
                //Parametrage par defaut de bC
                BonCommandeFournisseurModel parametreBC;
                var parametrageBCFournisseur = parametrageRepository.GetParametrageDocument();
                if (parametrageBCFournisseur.Type == (int)TypeParametrage.parametrageDocument) {
                    parametreBC = JsonConvert.DeserializeObject<BonCommandeFournisseurModel>(parametrageBCFournisseur.Contenu);

                    bc.DateExpiration = DateTime.Now.AddDays(parametreBC.validite_boncommande);
                    bc.ConditionsReglement = parametreBC.conditions_boncommande;
                    bc.note = parametreBC.note_boncommande;
                    bc.Objet = parametreBC.objet_boncommande;

                };
                List<HistoriqueModel> hitoriques = new List<HistoriqueModel>();
                var userManager = new UserManager(DbContext);
                var ccurrentUser = userManager.GetCurrentUser();
               
                hitoriques.Add(new HistoriqueModel()
                {
                    date = DateTime.Now,
                    action = (int)ActionHistorique.Added,
                    IdUser = ccurrentUser.Id,
                    champs = new List<ModifyEntryModel>(),
                });
                bc.Historique = JsonConvert.SerializeObject(hitoriques);
                bc.IdChantier = devis.IdChantier;
                bc.IdDevis = devis.Id;
                bc.IdFournisseur = idfournis;
             
                bc.Memos = "[]";
               
                bc.Status = (int)StatutBonCommandeFournisseur.Brouillon;
                bc.Total = listTtc.Sum();
                bc.TotalHt = list.Sum();
                bc.Tva = JsonConvert.SerializeObject(result); 
                bc.Fournisseur = null;
                bc.Chantier = devis.Chantier;
                bc.DateCreation = DateTime.Now;
                bc.DepenseBonCommandeFournisseurs = null;
                bc.Devis = devis;
                return bc;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public bool supprimerDevis(int id)
        {
            try
            {
                //vérifier que devis pas de prestation
                if (DbContext.BonCommandeFournisseur.Where(PF => PF.IdDevis == id).Count() != 0)
                {
                    return false;
                }
                if (DbContext.Factures.Where(PF => PF.IdDevis == id).Count() != 0)
                {
                    return false;
                }
               
                //supprimer devis
                var devis = DbContext.Devis.SingleOrDefault(F => F.Id == id);
                DbContext.Devis.Remove(devis);
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
