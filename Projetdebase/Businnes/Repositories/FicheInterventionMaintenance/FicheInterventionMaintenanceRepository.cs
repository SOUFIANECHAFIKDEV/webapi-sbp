using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.FicheInterventionMaintenance
{
    public class FicheInterventionMaintenanceRepository : EntityFrameworkRepository<Entities.FicheInterventionMaintenance, int>, IFicheInterventionMaintenanceRepository
    {
        public ProjetBaseContext SbpContext;
        public FicheInterventionMaintenanceRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            SbpContext = dbContext;
        }

        public Entities.FicheInterventionMaintenance GetFicheInterventionMaintenance(int id)
        {
            try
            {
                var ficheInterventionMaintenance = DbContext.FicheInterventionMaintenance.Where(x => x.Id == id)
                                                     .Include(x => x.VisiteMaintenance).ThenInclude(x => x.ContratEntretien).ThenInclude(x => x.Client)
                                                     //.Include(x => x.Facture)
                                                     .Include(x => x.InterventionTechnicienMaintenance).ThenInclude(x => x.IdFicheInterventionMaintenanceNavigation)
                                                     //.Include(x => x.InterventionTechnicien).ThenInclude(x =>x.Tech)
                                                     .FirstOrDefault();
                return ficheInterventionMaintenance;

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
                    if (h.Attribute != "InterventionTechnicien" && h.Attribute != "Historique" && h.Attribute != "Chantier" && h.Attribute != "idAgendaGoogle" && h.Attribute != "Facture" && h.Attribute != "Memos" && h.Attribute != "Prestations" && h.Attribute != "Tva" && h.Attribute != "Emails" && h.Attribute != "DateCreation")
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
    }
}
