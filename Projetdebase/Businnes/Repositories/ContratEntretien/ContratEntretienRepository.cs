using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Models;
using ProjetBase.Businnes.Outils.EXCEL;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.ContratEntretien
{
    public class ContratEntretienRepository : EntityFrameworkRepository<Entities.ContratEntretien, int>, IContratEntretienRepository
    {
        private readonly IGenerateExcelGammeMaintenance generateExcelGammeMaintenance;
        public ContratEntretienRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            generateExcelGammeMaintenance = new GenerateExcelGammeMaintenance(dbContext);
        }

        public async Task<List<EquipementContrat>> AddEquipementContrat(int id, List<EquipementContrat> equipementContrat)
        {
            var ListEquipements = new List<EquipementContrat>();
            foreach (var equipement in equipementContrat)
            {
                var newEquipementContrat = new EquipementContrat
                {
                    idContrat = id,
                    Nom = equipement.Nom
                };
                DbContext.EquipementContrat.Add(newEquipementContrat);

                foreach (var libelle in equipement.Libelle)
                {
                    var newLibelleEquipement = new LibelleEquipement
                    {
                        idEquipementContrat = newEquipementContrat.Id,
                        Nom = libelle.Nom
                    };
                    DbContext.Libelle.Add(newLibelleEquipement);

                    foreach (var operation in libelle.OperationsEquipement)
                    {
                        var newOperationEquipement = new OperationsEquipement
                        {
                            idLotEquipement = newLibelleEquipement.Id,
                            Nom = operation.Nom,
                        };
                        DbContext.OperationsEquipement.Add(newOperationEquipement);

                        foreach (var periode in operation.Periodicite)
                        {
                            var newPeriodiciteEquipement = new PeriodiciteEquipement
                            {
                                idOperationsEquipement = newOperationEquipement.Id,
                                Mois = periode.Mois,
                                statut = periode.statut,
                                Observation = periode.Observation,
                            };
                            DbContext.Periodes.Add(newPeriodiciteEquipement);
                        }

                    }

                }
                ListEquipements.Add(newEquipementContrat);

            }
            await DbContext.SaveChangesAsync();

            return ListEquipements;
        }

        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {
            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();
                historiqueChamps.ForEach(h =>
                {
                    var dd = h.Attribute;
                    if (h.Attribute != "Historique" && h.Attribute != "Clinet" && h.Attribute != "Client" && h.Attribute != "Memos" && h.Attribute != "EquipementContrat")
                    {

                        if (h.Attribute == "IdClient")
                        {
                            h.Attribute = "Client";
                            int id = Convert.ToInt32(h.After);
                            h.After = DbContext.Clients.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;

                            id = Convert.ToInt32(h.Before);
                            h.Before = DbContext.Clients.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                        }

                        if (h.Attribute == "DateDebut")
                        {
                            h.Attribute = "Date Début";
                        }
                        if (h.Attribute == "DateFin")
                        {
                            h.Attribute = "Date Fin";
                        }
                        //if (h.Attribute == "Status")
                        //{

                        //    h.After = getLabelStatut(h.After);
                        //    h.Before = getLabelStatut(h.Before);
                        //}
                        if (h.Attribute == "Site")
                        {
                            h.Attribute = "Site";
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

        public async Task<bool> deleteEquipementContrat(List<EquipementContrat> equipementContrat)
        {
            DbContext.RemoveRange(equipementContrat);
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<byte[]> ExportGammeMaintenenceEquipement(Entities.ContratEntretien contratEntretien)
        {
            //  var x = generateExcelGammeMaintenance.GenerateJournalBanqueFile();
            return generateExcelGammeMaintenance.GenerateGammeMaintenenceEquipement(contratEntretien);
        }

        public Entities.ContratEntretien GetContratEntretien(int id)
        {
            try
            {
                var ContratEntretien = DbContext.ContratEntretien.Where(x => x.Id == id)
                                        .Include(x => x.Client)
                                         .Include(x => x.VisiteMaintenance).ThenInclude(x => x.FicheInterventionMaintenance)
                                        .Include(x => x.EquipementContrat).ThenInclude(x => x.Libelle).ThenInclude(x => x.OperationsEquipement).ThenInclude(x => x.Periodicite)
                                        .FirstOrDefault();
                return ContratEntretien;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }

        public async Task<bool> saveMemos(int id, string memos)
        {
            try
            {
                var contratEntretien = GetById(id);
                contratEntretien.Memos = memos;
                Update(contratEntretien);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
    }
}
