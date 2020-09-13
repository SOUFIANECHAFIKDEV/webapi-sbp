using InoAuthentification.Entities;
using Newtonsoft.Json;
using ProjetBase.Businnes.Contexts;
using ProjetBase.Businnes.Enum;
using ProjetBase.Businnes.Models;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ProjetBase.Businnes.Extensions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using InoAuthentification.UserManagers;
using ProjetBase.Businnes.Repositories.BonCommandeFournisseur;
using ProjetBase.Businnes.Entities;

namespace ProjetBase.Businnes.Repositories.BonCommandeFournisseur
{
    public class BonCommandeFournisseurtRepository : EntityFrameworkRepository<Entities.BonCommandeFournisseur, int>, IBonCommandeFournisseur
    {

        public ProjetBaseContext ProjetBaseContext;

        public BonCommandeFournisseurtRepository(ProjetBaseContext dbContext) : base(dbContext)
        {
            ProjetBaseContext = dbContext;
        }

        public bool CheckUniqueReference(string reference)
        {
            var NbrReference = DbContext.BonCommandeFournisseur.Where(x => x.Reference == reference).Count();
            return NbrReference > 0;
        }

        /*
         * Save Memos de client
         */
        public async Task<bool> saveMemos(int id, string memos)
        {
            try
            {
                var client = GetById(id);
                client.Memos = memos;
                Update(client);
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        /*
         *  Changer les ids à des noms
         */
        public List<ModifyEntryModel> ChangeIdToNameHistorique(List<ModifyEntryModel> historiqueChamps)
        {

            try
            {
                List<ModifyEntryModel> NewhistoriqueChamps = new List<ModifyEntryModel>();
                historiqueChamps.ForEach(h =>

                {
                    var dd = h.Attribute;
                    if (h.Attribute != "Articles"  && h.Attribute != "Chantier"   && h.Attribute != "Tva" && h.Attribute != "Historique"  && h.Attribute != "DateCreation" && h.Attribute != "Memos" &&  h.Attribute != "Prestations" && h.Attribute != "Devis" && h.Attribute != "Fournisseur" && h.Attribute != "DepenseBonCommandeFournisseurs")
                    {
                        if (h.Attribute == "IdChantier")
                        {
                            h.Attribute = "Chantier";
                            int id = Convert.ToInt32(h.After);
                            h.After = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;

                            id = Convert.ToInt32(h.Before);
                            h.Before = DbContext.Chantier.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                        }
                        if (h.Attribute == "IdFournisseur")
                        {
                            h.Attribute = "Fournisseur";
                            int id = Convert.ToInt32(h.After);
                            h.After = DbContext.Fournisseurs.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;

                            id = Convert.ToInt32(h.Before);
                            h.Before = DbContext.Fournisseurs.Where(C => C.Id.Equals(id)).SingleOrDefault().Nom;
                        }

                        if (h.Attribute == "Status")
                        {
                            h.After = getStatusBonCommandeFournisseur(Int32.Parse(h.After));
                            h.Before = getStatusBonCommandeFournisseur(Int32.Parse(h.Before));

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

        // Get statut à partir de son numéro
        public string getStatusBonCommandeFournisseur(int v)
        {
            try
            {
                var statut = "";
                switch (v)
                {
                    case (int)StatutBonCommandeFournisseur.Brouillon:
                        statut = "Brouillon";
                        break;
                    case (int)StatutBonCommandeFournisseur.Encours:
                        statut = "En cours";
                        break;
                    case (int)StatutBonCommandeFournisseur.Facturee:
                        statut = "Facturée";
                        break;
                 
                    case (int)StatutBonCommandeFournisseur.Annule:
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

        public Entities.BonCommandeFournisseur GetBonCommandeFournisseur(int id)
        {
            try
            {
                var bonCommandeFournisseur = DbContext.BonCommandeFournisseur.Where(x => x.Id == id)
                                        .Include(x => x.Chantier).ThenInclude(x => x.Client)
                                        .Include(x=> x.Devis)
                                        .Include(x => x.DepenseBonCommandeFournisseurs).ThenInclude(x => x.Depense)
                                        .Include(x => x.Fournisseur)
                                        .FirstOrDefault();
                return bonCommandeFournisseur;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw ex;
            }
        }
    }

}
