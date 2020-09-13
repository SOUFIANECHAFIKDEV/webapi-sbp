using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InoMessagerie.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Extensions;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Models.Paging;
using ProjetBase.Businnes.Models.Sorting;
using ProjetBase.Businnes.Outils.Messagerie;
using ProjetBase.Businnes.Outils.SynchroAgenda;
using Serilog;

namespace ProjetBase.Businnes.Repositories.FicheIntervention
{
    public class FicheInterventionRepository : EntityFrameworkRepository<Entities.FicheIntervention, int>, IFicheInterventionRepository
    {

        private readonly IAgenda _agenda;
        public ProjetBaseContext SbpContext;
        public FicheInterventionRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            SbpContext = dbContext;
            _agenda = new AgendaGoogle();
        }

        public string AddEventToGoogleAgenda(Entities.FicheIntervention ficheIntervention)
        {

            try
            {
                string id = null;
                var parametrageTypeAgenda = DbContext.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.syncAgendaGoogle)).FirstOrDefault();
                var parametrage = parametrageTypeAgenda.Contenu;
                ParametrageAgendaGoogle contenu = JsonConvert.DeserializeObject<ParametrageAgendaGoogle>(parametrageTypeAgenda.Contenu);
                if (!string.IsNullOrEmpty(contenu.applicationName) && !string.IsNullOrEmpty(contenu.clientId) && !string.IsNullOrEmpty(contenu.clientSecret) && !string.IsNullOrEmpty(contenu.calendarId))
                {
                    id =  _agenda.AddeventCalendarGroup(
                        contenu.applicationName,
                        contenu.clientId,
                        contenu.clientSecret,
                        contenu.calendarId,
                        "Fiche Intervention " + ficheIntervention.Reference,
                        ficheIntervention.DateDebut,
                         ficheIntervention.DateFin
                    );
                }
                return id;
            }
            catch (Exception)
            {
                return null;
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
                    if (h.Attribute != "InterventionTechnicien" && h.Attribute != "Historique" && h.Attribute != "Chantier" && h.Attribute != "idAgendaGoogle" && h.Attribute != "Facture" && h.Attribute != "Memos" && h.Attribute != "Prestations" && h.Attribute != "Tva"  && h.Attribute != "Emails" && h.Attribute != "DateCreation")
                    {
                        
                        if (h.Attribute == "IdChantier")
                        {
                            h.Attribute = "Chantier";
                            int id = Convert.ToInt32(h.After);
                            h.After = SbpContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;

                            id = Convert.ToInt32(h.Before);
                            h.Before = SbpContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                        }
                        if (h.Attribute == "IdFacture")
                        {
                            h.Attribute = "Facture";
                            int id = Convert.ToInt32(h.After);
                            h.After = SbpContext.Factures.Where(C => C.Id.Equals(id)).SingleOrDefault().Reference;

                            id = Convert.ToInt32(h.Before);
                            h.Before = SbpContext.Factures.Where(C => C.Id.Equals(id)).SingleOrDefault().Reference;
                        }
                        if (h.Attribute == "DateDebut")
                        {
                            h.Attribute = "Date Début";
                        }
                        if (h.Attribute == "DateFin")
                        {
                            h.Attribute = "Date Fin";
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
                                var adresseville= AdresseIntervention.ville;
                               // var adressepays = AdresseIntervention.pays;
                                var htmladressedesignation = "<strong> Désignation : </strong>" + adressedesignation + "</br>";
                                var htmladressedresse = "<strong>  Complément Adresset : </strong>" + adresseadresse + "</br>";
                                var htmladressecomplementAdresse = "<strong>  Adresse  : </strong>" + adressecomplementAdresse + "</br>";
                                var htmladresseville = "<strong>  Ville : </strong>" + adresseville + "</br>";
                                var htmladressedepartement = "<strong>  Département : </strong>" + adressedepartement + "</br>";
                                var htmladressecodepostal = "<strong>  Code Postal : </strong>" + adressecodePostal + "</br>";
                                //var htmladressepays = "<strong>  Code Postal : </strong>" + adressepays + "</br>";
                                h.Before = htmladressedesignation + htmladressedresse + htmladressecomplementAdresse + htmladresseville + htmladressecodepostal + htmladressedepartement ;

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
                                h.After = htmladressedesignation + htmladressedresse + htmladressecomplementAdresse  + htmladresseville + htmladressecodepostal + htmladressedepartement;

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
        private string getLabelStatut(string statut)
        {
            switch (statut)
            {
                case "Brouillon":
                    return "Brouillon";
                case "Planifiee":
                    return "Planifiée";
                case "Realisee":
                    return "Réalisée";
                case "Annulee":
                    return "Annulée";
                case "Facturee":
                    return "Facturée";
                default:
                    return "";
            }
        }

        public bool CheckUniqueReference(string reference)
        {
            var NbrReference = DbContext.FicheIntervention.Where(x => x.Reference == reference).Count();
            return NbrReference > 0;
        }

        public Entities.FicheIntervention GetFicheIntervention(int id)
        {
            try
            {
                var ficheIntervention = DbContext.FicheIntervention.Where(x => x.Id == id)
                                                     .Include(x => x.Chantier).ThenInclude(x => x.Client)
                                                     .Include(x=> x.Facture)
                                                     .Include(x => x.InterventionTechnicien).ThenInclude(x => x.IdTechnicienNavigation)
                                                     //.Include(x => x.InterventionTechnicien).ThenInclude(x =>x.Tech)
                                                     .FirstOrDefault();
                return ficheIntervention;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<MessagerResponseModel> SendEmail(int idFicheIntervention, InoMessagerie.Models.SendMailParamsModel MailParams)
        {
            try
            {
                Messagerie messagerie = new Messagerie(DbContext);

                MessagerResponseModel result = messagerie.SendEmail(MailParams);
                if (result.statut == 200)
                {
                    var currentUser = EntityExtensions.GetCurrentUser(DbContext);
                    Entities.FicheIntervention ficheIntervention = DbContext.FicheIntervention.Where(F => F.Id == idFicheIntervention).FirstOrDefault();
                    // var Emails = JsonConvert.DeserializeObject<List<object>>(devis.Emails);
                    if (ficheIntervention.Emails != "" && ficheIntervention.Emails != null)
                    {
                        var Emails = JsonConvert.DeserializeObject<List<object>>(ficheIntervention.Emails);
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
                        ficheIntervention.Emails = JsonConvert.SerializeObject(Emails);
                    }
                    else
                    {
                        ficheIntervention.Emails = JsonConvert.SerializeObject(ficheIntervention.Emails);
                    }


                    Update(ficheIntervention);
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

        /*
 * Save Memos de fiche intervention
 */
        public async Task<bool> saveMemos(int id, string memos)
        {
            try
            {
                var fiicheIntervention = GetById(id);
                fiicheIntervention.Memos = memos;
                Update(fiicheIntervention);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public string UpdateEventToGoogleAgenda(Entities.FicheIntervention ficheIntervention)
        {
            try
            {

                var parametrageTypeAgenda = DbContext.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.syncAgendaGoogle)).FirstOrDefault();
                var parametrage = parametrageTypeAgenda.Contenu;
                ParametrageAgendaGoogle contenu = JsonConvert.DeserializeObject<ParametrageAgendaGoogle>(parametrageTypeAgenda.Contenu);
                ///var technicien = await DbContext.User.Where(x => x.Id == dp.IdTechnicien).FirstOrDefaultAsync();
                //var nomClient = await DbContext.Clients.Where(x => x.Id == dp.IdClient).Select(x => x.Nom).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(contenu.applicationName) && !string.IsNullOrEmpty(contenu.clientId) && !string.IsNullOrEmpty(contenu.clientSecret) && !string.IsNullOrEmpty(contenu.calendarId))
                {
                    _agenda.UpdateeventCalendar(
                       contenu.applicationName,
                       contenu.clientId,
                       contenu.clientSecret,
                       contenu.calendarId,
                       "Fiche Intervention " + ficheIntervention.Reference,
                        // "Client : " + nomClient + " - Technicien : " + technicien.Nom + " " + technicien.Prenom,
                        ficheIntervention.DateDebut,
                        ficheIntervention.DateFin,
                        ficheIntervention.idAgendaGoogle
                   // dp.DateIntervention.AddHours(1)
                   );
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string DeleteEventToGoogleAgenda(Entities.FicheIntervention ficheIntervention)
        {
            try
            {

                var parametrageTypeAgenda = DbContext.Parametrages.Where(p => p.Type == Convert.ToInt32(TypeParametrage.syncAgendaGoogle)).FirstOrDefault();
                var parametrage = parametrageTypeAgenda.Contenu;
                ParametrageAgendaGoogle contenu = JsonConvert.DeserializeObject<ParametrageAgendaGoogle>(parametrageTypeAgenda.Contenu);
                ///var technicien = await DbContext.User.Where(x => x.Id == dp.IdTechnicien).FirstOrDefaultAsync();
                //var nomClient = await DbContext.Clients.Where(x => x.Id == dp.IdClient).Select(x => x.Nom).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(contenu.applicationName) && !string.IsNullOrEmpty(contenu.clientId) && !string.IsNullOrEmpty(contenu.clientSecret) && !string.IsNullOrEmpty(contenu.calendarId))
                {
                    _agenda.DeleteeventCalendar(
                       contenu.applicationName,
                       contenu.clientId,
                       contenu.clientSecret,
                       contenu.calendarId,
                       "Fiche Intervention " + ficheIntervention.Reference,
                        // "Client : " + nomClient + " - Technicien : " + technicien.Nom + " " + technicien.Prenom,
                        ficheIntervention.DateDebut,
                        ficheIntervention.DateFin,
                        ficheIntervention.idAgendaGoogle
                   // dp.DateIntervention.AddHours(1)
                   );
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<List<Entities.FicheIntervention>> GetFicheInterventionsChantier(int IdChantier)
        {
            List<Entities.FicheIntervention> FicheInterventionsList = new List<Entities.FicheIntervention>();
            List<Entities.Chantier> Chantiers = DbContext.Chantier.Where(c => c.Id == IdChantier).ToList();
            if (Chantiers.Count > 0)
            {
                foreach (var item in Chantiers)
                {
                    List<Entities.FicheIntervention> internalInvoices = DbContext.FicheIntervention.Where(f => f.IdChantier == item.Id).ToList();
                    if (internalInvoices.Count > 0)
                    {
                        FicheInterventionsList = FicheInterventionsList.Concat(internalInvoices).ToList();
                    }
                }
            }
            var listFacture = FicheInterventionsList.Where(x => x.Status == StatutFicheIntervention.Realisee).ToList();
            return Task.FromResult(listFacture);





        }


        public PagedList<Entities.FicheIntervention> Filter(PagingParams pagingParams, Expression<Func<Entities.FicheIntervention, bool>> filter = null, SortingParams sortingParams = null)
        {

            IQueryable<Entities.FicheIntervention> query = SbpContext.Set<Entities.FicheIntervention>();
            query = query.Where(filter).Include(C => C.Chantier).ThenInclude(C => C.Client).Include(x => x.InterventionTechnicien).ThenInclude(C => C.IdTechnicienNavigation);
            if (sortingParams != null)
            {
                query = new SortedList<Entities.FicheIntervention>(query, sortingParams).GetSortedList();
            }

            return new PagedList<Entities.FicheIntervention>(query, pagingParams.PageNumber, pagingParams.PageSize);
        }
    }
}
